using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XiaoXu.Core;
using System.Collections.Generic;

public class LoadTest : MonoBehaviour
{
	[Header("标签")]
	public string label;

	[Header("图片")]
	public Image targetImage;
	public string imageSpriteAddress;

	[Header("敌人")]
	public string enemyName;

    [Header("实例化设置")]
	public Transform spawnParent;
	public Vector3 spawnPosition = Vector3.zero;

	private GameMain gameMain;
	private ResourceManager resourceManager;
	private List<GameObject> spawnedObjects = new List<GameObject>();

	async void Start()
	{
		gameMain = GetComponent<GameMain>();
		resourceManager = gameMain.GetComponentInChildren<ResourceManager>();

		await LoadAndSpawnByLabel();
		await LoadAndSetSprite();
		await LoadAndSpawnByName();
	}

	private async Task LoadAndSpawnByLabel()
	{
		List<GameObject> loadedPrefabs = await resourceManager.LoadAssetsByLabelAsync<GameObject>(label);
		Debug.Log($"通过标签 {label} 加载了 {loadedPrefabs.Count} 个GameObject");

		foreach (GameObject prefab in loadedPrefabs)
		{
			if (prefab != null)
			{
				GameObject instance = await resourceManager.InstantiateAsync(prefab.name,spawnPosition,Quaternion.identity);

				if (instance != null)
				{
					if (spawnParent != null)
					{
						instance.transform.SetParent(spawnParent, false);
					}

					spawnedObjects.Add(instance);
				}
			}
		}
	}
	private async Task LoadAndSpawnByName()
	{
        GameObject obj = await resourceManager.InstantiateAsync(enemyName, spawnPosition, Quaternion.identity);
	}
	private async Task LoadAndSetSprite()
	{
		Sprite loadedSprite = await resourceManager.LoadAssetAsync<Sprite>(imageSpriteAddress);

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
		foreach (GameObject obj in spawnedObjects)
		{
			if (obj != null)
			{
				resourceManager.DestroyInstance(obj);
			}
		}
		spawnedObjects.Clear();

		// 释放图片资源
		if (!string.IsNullOrEmpty(imageSpriteAddress))
		{
			resourceManager.ReleaseAsset(imageSpriteAddress);
		}

		// 释放标签资源
		resourceManager.ReleaseAssetsByLabel(label);

		Debug.Log("LoadTest 资源清理完成");
	}
}