namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;

	class FaceDetectorScene : WebCamera
	{
		public TextAsset faces;
		public double StabilizationThreshold = 2.0;
		public int StabilizationSampleCount = 2;

		private FaceProcessorLive<WebCamTexture> processor;

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, null, null);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = StabilizationThreshold;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = StabilizationSampleCount;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			// detect everything we're interested in
			processor.ProcessTexture(input, TextureParameters);

			if(processor.DataStabilizer.Threshold != StabilizationThreshold){
				processor.DataStabilizer.Threshold = StabilizationThreshold;
			}
			if(processor.DataStabilizer.SamplesCount != StabilizationSampleCount){
				processor.DataStabilizer.SamplesCount = StabilizationSampleCount;
			}

			// mark detected objects
			processor.MarkDetected();

			// processor.Image now holds data we'd like to visualize
			output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

			return true;
		}

		public List<DetectedFace> Faces(){
			return processor.Faces;
		}
	}
}