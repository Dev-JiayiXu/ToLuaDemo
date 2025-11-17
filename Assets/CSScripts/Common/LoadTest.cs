using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XiaoXu.Core;

public class LoadTest : MonoBehaviour
{
    //加载一个gameobject和设置一张sprite
    public Image targetImage;
    public string gameObjectAddress;
    public string imageSpriteAddress;

    private GameMain gameMain;
    private ResLoadManager _resManager;
    private GameObject _spawnedGameObject;

    async void Start()
    {
        gameMain = GetComponent<GameMain>();
        _resManager = gameMain.GetComponentInChildren<ResLoadManager>();

        await LoadAndSpawnGameObject();
        await LoadAndSetSprite();
        await LoadBothSimultaneously();
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

        await Task.WhenAll(gameObjectTask, spriteTask);

        _spawnedGameObject = gameObjectTask.Result;
        if (spriteTask.Result != null && targetImage != null)
        {
            targetImage.sprite = spriteTask.Result;
        }
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