# Unity `MediaPlayer` component

The [`MediaPlayer`](xref:Microsoft.MixedReality.WebRTC.Unity.MediaPlayer) Unity component is a utility component mixing an audio source and a video source for local rendering.

> [!Important]
> FIXME: This component is not currently fully functional. The remote audio data is currently sent directly to the local audio out device by the internal WebRTC implementation without any configuration possible. And the local audio is never rendered. So the [`MediaPlayer.AudioSource`](xref:Microsoft.MixedReality.WebRTC.Unity.MediaPlayer.AudioSource) property is unused. The video however capability works as intended. See issue [#92](https://github.com/microsoft/MixedReality-WebRTC/issues/92) for details.

![The MediaPlayer Unity component](unity-mediaplayer.png)

| Property | Description |
|---|---|
| **Source** | |
| AudioSource | Reference to the [`AudioSender`](xref:Microsoft.MixedReality.WebRTC.Unity.AudioSender) or [`AudioReceiver`](xref:Microsoft.MixedReality.WebRTC.Unity.AudioSender) instance that the media player outputs the audio of. This property is of type [`MediaSource`](xref:Microsoft.MixedReality.WebRTC.Unity.MediaSource), but only instances of classes implementing the [`IAudioSource`](xref:Microsoft.MixedReality.WebRTC.Unity.IAudioSource) interface are valid. |
| VideoSource | Reference to the [`VideoSender`](xref:Microsoft.MixedReality.WebRTC.Unity.VideoSender) or [`VideoReceiver`](xref:Microsoft.MixedReality.WebRTC.Unity.VideoReceiver) instance that the media player renders the video of. This property is of type [`MediaSource`](xref:Microsoft.MixedReality.WebRTC.Unity.MediaSource), but only instances of classes implementing the [`IVideoSource`](xref:Microsoft.MixedReality.WebRTC.Unity.IVideoSource) interface are valid. |
| MaxVideoFramerate | Maximum number of video frames per second rendered. Extra frames coming from the video source are discarded. |
| **Statistics** | |
| EnableStatistics | Enable the collecting of video statistics by the media player. This adds some minor overhead. |
| FrameLoadStatHolder | Reference to a [`TextMesh`](https://docs.unity3d.com/ScriptReference/TextMesh.html) instance whose text is set to the number of incoming video frames per second pulled from the video source into the media player's internal queue. |
| FramePresentStatHolder | Reference to a [`TextMesh`](https://docs.unity3d.com/ScriptReference/TextMesh.html) instance whose text is set to the number of video frames per second dequeued from the media player's internal queue and rendered to the texture(s) of the [`Renderer`](https://docs.unity3d.com/ScriptReference/Renderer.html) component associated with this media player. |
| FrameSkipStatHolder | Reference to a [`TextMesh`](https://docs.unity3d.com/ScriptReference/TextMesh.html) instance whose text is set to the number of video frames per second dropped due to the media player's internal queue being full. This corresponds to frames being enqueued faster than they are dequeued, which happens when the source is overflowing the sink, and the sink cannot render all frames. |
