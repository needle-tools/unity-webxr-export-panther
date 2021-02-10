#if AR_FOUNDATION_INSTALLED

using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace WebXR
{
	public class WebXRImageTrackingSubsystem : XRImageTrackingSubsystem
	{
		public const string SubsystemId = "WebXR-ImageTrackingSubsystem";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RegisterDescriptor()
		{
#if UNITY_WEBGL
			XRImageTrackingSubsystemDescriptor.Create(new XRImageTrackingSubsystemDescriptor.Cinfo
			{
				id = SubsystemId,
#if !UNITY_2019_4
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRImageTrackingSubsystem),
#endif
				supportsMovingImages = true,
				supportsMutableLibrary = true
			});
#endif
		}

		protected override void OnStart()
		{
			if(imageLibrary == null)
				imageLibrary = CreateRuntimeLibrary(null);

			if (imageLibrary is WebXRImageLibrary lib)
			{
				var manager = Object.FindObjectOfType<ARTrackedImageManager>();
				if (manager && manager.referenceLibrary != null)
				{
					lib.Add(manager.referenceLibrary);
				}
			}

			base.OnStart();
		}

		private class XRProvider : Provider
		{
			private RuntimeReferenceImageLibrary library;

#if !UNITY_2019_4
			public override void Start()
			{
			}

			public override void Stop()
			{
			}
#endif

			public override void Destroy()
			{
			}

			public override TrackableChanges<XRTrackedImage> GetChanges(XRTrackedImage defaultTrackedImage, Allocator allocator)
			{
				// TODO: implement
				return new TrackableChanges<XRTrackedImage>();
			}

			public override RuntimeReferenceImageLibrary imageLibrary
			{
				set => library = value;
			}

			public override RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary)
			{
				if (library == null) library = new WebXRImageLibrary(serializedLibrary);
				return library;
			}
		}
#if UNITY_2019_4
		protected override Provider CreateProvider()
		{
			return new XRProvider();
		}
#endif

		private class WebXRImageLibrary : MutableRuntimeReferenceImageLibrary
		{
			private readonly List<XRReferenceImage> _images = new List<XRReferenceImage>();


			public WebXRImageLibrary(IReferenceImageLibrary serializedLibrary)
			{
				Add(serializedLibrary);
			}

			public void Add(IReferenceImageLibrary serializedLibrary)
			{
				if (serializedLibrary != null)
				{
					for (var i = 0; i < serializedLibrary.count; i++)
					{
						var img = serializedLibrary[i];
						this._images.Add(img);
					}
				}
			}

			public override int count => _images.Count;

			protected override XRReferenceImage GetReferenceImageAt(int index)
			{
				return _images[index];
			}

			protected override JobHandle ScheduleAddImageJobImpl(NativeSlice<byte> imageBytes, Vector2Int sizeInPixels, TextureFormat format,
				XRReferenceImage referenceImage, JobHandle inputDeps)
			{
				if (this._images.Contains(referenceImage)) return new JobHandle();
				this._images.Add(referenceImage);
				return new JobHandle();
			}


			public override int supportedTextureFormatCount => k_SupportedFormats.Length;

			protected override TextureFormat GetSupportedTextureFormatAtImpl(int index) => k_SupportedFormats[index];

			// TODO: which are actually supported?
			private static readonly TextureFormat[] k_SupportedFormats =
			{
				TextureFormat.Alpha8,
				TextureFormat.R8,
				TextureFormat.RFloat,
				TextureFormat.RGB24,
				TextureFormat.RGBA32,
				TextureFormat.ARGB32,
				TextureFormat.BGRA32,
			};
		}
	}
}

#endif