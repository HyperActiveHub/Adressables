using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
	public AssetReferenceAudioClip(string guid): base(guid) { }
}

public class AdressablesManager : MonoBehaviour
{
	[SerializeField] AssetReference playerAssetReference;

	[SerializeField] AssetReferenceAudioClip musicAssetReference;

	[SerializeField] AssetReferenceTexture2D logoAssetReference;

	RawImage rawImageLogo;

	GameObject playerController;

	void Start()
    {
		Addressables.InitializeAsync().Completed += AdressablesManager_Completed;	//will be called automatically otherwise (?), this provides more control. 
	}

	void AdressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
	{
		playerAssetReference.InstantiateAsync().Completed += (go) =>
		{
			var player = go.Result;
			playerController = player;
			//do stuff to the prefab, ex. camera follow on player
		};

		musicAssetReference.LoadAssetAsync<AudioClip>().Completed += (clip) =>
		{
			var aSource = gameObject.AddComponent<AudioSource>();
			aSource.clip = clip.Result;
			aSource.playOnAwake = false;
			aSource.loop = true;
			aSource.Play();
		};

		logoAssetReference.LoadAssetAsync<Texture2D>();

	}

    void Update()
    {
        if(logoAssetReference.Asset != null && rawImageLogo.texture == null)	//not recommended, but used by unity?
		{
			rawImageLogo.texture = logoAssetReference.Asset as Texture2D;	//alpha is 0.
			Color currentColor = rawImageLogo.color;
			currentColor.a = 1.0f;
			rawImageLogo.color = currentColor;

		}
    }

	void OnDestroy()
	{
		//can unload stuff to memory manage
		playerAssetReference.ReleaseInstance(playerController);	
		logoAssetReference.ReleaseAsset();
	}
}
