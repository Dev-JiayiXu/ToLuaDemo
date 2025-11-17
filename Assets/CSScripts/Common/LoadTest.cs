using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XiaoXu.Core;

public class LoadTest : MonoBehaviour
{
    
    public Image targetImage;
    public string gameObjectAddress = "GameObject";
    public string imageSpriteAddress = "Sprite";

    private GameMain gameMain;
    private ResLoadManager _resManager;
    private GameObject _spawnedGameObject;

    async void Start()
    {
        gameMain = GetComponent<GameMain>();
        // 获取资源管理器
        _resManager = gameMain.GetComponentInChildren<ResLoadManager>();

        // 加载资源
        await LoadAndSpawnGameObject();
        await LoadAndSetSprite();

        // 同时加载两个资源
        // await LoadBothSimultaneously();
    }

    private async Task LoadAndSpawnGameObject()
    {
        _spawnedGameObject = await _resManager.InstantiateAsync(
            gameObjectAddress,
            Vector3.zero,
            Quaternion.identity
        );
    }

    private async Task LoadAndSetSprite()
    {
        Sprite loadedSprite = await _resManager.LoadAssetAsync<Sprite>(imageSpriteAddress);

        if (loadedSprite != null && targetImage != null)
        {
            targetImage.sprite = loadedSprite;
        }
    }

    private async Task LoadBothSimultaneously()
    {
        var gameObjectTask = _resManager.InstantiateAsync(gameObjectAddress, Vector3.zero, Quaternion.identity);
        var spriteTask = _resManager.LoadAssetAsync<Sprite>(imageSpriteAddress);

        // 等待两个都完成
        await Task.WhenAll(gameObjectTask, spriteTask);

        // 处理结果
        _spawnedGameObject = gameObjectTask.Result;
        if (spriteTask.Result != null && targetImage != null)
        {
            targetImage.sprite = spriteTask.Result;
        }

        Debug.Log("所有资源加载完成！");
    }

    void OnDestroy()
    {
        if (_spawnedGameObject != null)
        {
            _resManager.ReleaseInstance(_spawnedGameObject);
        }

        _resManager.ReleaseAsset(imageSpriteAddress);
        _resManager.ReleaseAsset(gameObjectAddress);
    }
}