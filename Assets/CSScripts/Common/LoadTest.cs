using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XiaoXu.Core;
using System.Collections.Generic;

public class LoadTest : MonoBehaviour
{
	[Header("标签")]
	public string labelToLoad;

	[Header("图片")]
	public Image targetImage;
	public string imageSpriteAddress;

	[Header("实例化设置")]
	public Transform spawnParent; // 可选的父物体
	public Vector3 spawnPosition = Vector3.zero;

	private GameMain gameMain;
	private ResourceManager _resourceManager;
	private List<GameObject> _spawnedObjects = new List<GameObject>();

	async void Start()
	{
		gameMain = GetComponent<GameMain>();
		_resourceManager = gameMain.GetComponentInChildren<ResourceManager>();

		await LoadAndSpawnByLabel();
		await LoadAndSetSprite();
	}

	private async Task LoadAndSpawnByLabel()
	{
		List<GameObject> loadedPrefabs = await _resourceManager.LoadAssetsByLabelAsync<GameObject>(labelToLoad);
		Debug.Log($"通过标签 '{labelToLoad}' 加载了 {loadedPrefabs.Count} 个GameObject");
		foreach (GameObject prefab in loadedPrefabs)
		{
			if (prefab != null)
			{
				GameObject instance = await _resourceManager.InstantiateAsync(
					prefab.name,
					spawnPosition,
					Quaternion.identity
				);

				if (instance != null)
				{
					if (spawnParent != null)
					{
						instance.transform.SetParent(spawnParent, false);
					}

					_spawnedObjects.Add(instance);
					Debug.Log($"实例化了: {prefab.name}");
				}
			}
		}

		Debug.Log($"总共实例化了 {_spawnedObjects.Count} 个对象");
	}

	private async Task LoadAndSetSprite()
	{
		Sprite loadedSprite = await _resourceManager.LoadAssetAsync<Sprite>(imageSpriteAddress);

		if (loadedSprite != null && targetImage != null)
		{
			targetImage.sprite = loadedSprite;
			targetImage.preserveAspect = true; // 保持图片比例
		}
		else
		{
			Debug.LogError($"加载图片失败: {imageSpriteAddress}");
		}
	}
	void OnDestroy()
	{
		// 销毁所有实例化的对象
		foreach (GameObject obj in _spawnedObjects)
		{
			if (obj != null)
			{
				_resourceManager.DestroyInstance(obj);
			}
		}
		_spawnedObjects.Clear();

		// 释放图片资源
		if (!string.IsNullOrEmpty(imageSpriteAddress))
		{
			_resourceManager.ReleaseAsset(imageSpriteAddress);
		}

		// 释放标签资源
		_resourceManager.ReleaseAssetsByLabel(labelToLoad);

		Debug.Log("LoadTest 资源清理完成");
	}
}