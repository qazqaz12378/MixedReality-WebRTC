// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;

namespace TestNetCoreConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Transceiver audioTransceiver = null;
            Transceiver videoTransceiver = null;
            LocalAudioTrack localAudioTrack = null;
            LocalVideoTrack localVideoTrack = null;

            try
            {
                bool needVideo = Array.Exists(args, arg => (arg == "-v") || (arg == "--video"));
                bool needAudio = Array.Exists(args, arg => (arg == "-a") || (arg == "--audio"));

                // Asynchronously retrieve a list of available video capture devices (webcams).
                var deviceList = await PeerConnection.GetVideoCaptureDevicesAsync();

                // For example, print them to the standard output
                foreach (var device in deviceList)
                {
                   Console.WriteLine($"Found webcam {device.name} (id: {device.id})");
                }

                // Create a new peer connection automatically disposed at the end of the program
                using var pc = new PeerConnection();

                // Initialize the connection with a STUN server to allow remote access
                var config = new PeerConnectionConfiguration
                {
                    IceServers = new List<IceServer> {
                            new IceServer{ Urls = { "stun:stun.l.google.com:19302" } }
                        }
                };
                await pc.InitializeAsync(config);
                Console.WriteLine("Peer connection initialized.");

                // Record video from local webcam, and send to remote peer
                if (needVideo)
                {
                    Console.WriteLine("Opening local webcam...");
                    localVideoTrack = await LocalVideoTrack.CreateFromDeviceAsync();
                    videoTransceiver = pc.AddTransceiver(MediaKind.Video);
                    videoTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;
                    videoTransceiver.LocalVideoTrack = localVideoTrack;
                }

                // Record audio from local microphone, and send to remote peer
                if (needAudio)
                {
                    Console.WriteLine("Opening local microphone...");
                    localAudioTrack = await LocalAudioTrack.CreateFromDeviceAsync();
                    audioTransceiver = pc.AddTransceiver(MediaKind.Audio);
                    audioTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;
                    audioTransceiver.LocalAudioTrack = localAudioTrack;
                }

                // Setup signaling
                Console.WriteLine("Starting signaling...");
                var signaler = new NamedPipeSignaler.NamedPipeSignaler(pc, "testpipe");
                signaler.SdpMessageReceived += async (SdpMessage message) => {
                    await pc.SetRemoteDescriptionAsync(message);
                    if (message.Type == SdpMessageType.Offer)
                    {
                        pc.CreateAnswer();
                    }
                };
                signaler.IceCandidateReceived += (IceCandidate candidate) => {
                    pc.AddIceCandidate(candidate);
                };
                await signaler.StartAsync();

                // Start peer connection
                pc.Connected += () => { Console.WriteLine("PeerConnection: connected."); };
                pc.IceStateChanged += (IceConnectionState newState) => { Console.WriteLine($"ICE state: {newState}"); };
                int numFrames = 0;
                pc.VideoTrackAdded += (RemoteVideoTrack track) =>
                {
                    track.I420AVideoFrameReady += (I420AVideoFrame frame) =>
                    {
                        ++numFrames;
                        if (numFrames % 60 == 0)
                        {
                            Console.WriteLine($"Received video frames: {numFrames}");
                        }
                    };
                };
                if (signaler.IsClient)
                {
                    Console.WriteLine("Connecting to remote peer...");
                    pc.CreateOffer();
                }
                else
                {
                    Console.WriteLine("Waiting for offer from remote peer...");
                }

                Console.WriteLine("Press a key to stop recording...");
                Console.ReadKey(true);

                signaler.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            localAudioTrack?.Dispose();
            localVideoTrack?.Dispose();

            Console.WriteLine("Program termined.");
        }
    }
}
