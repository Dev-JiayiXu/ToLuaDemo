--用Lua调用测试
---@class LoadTest
---@field label string
---@field targetImage UnityEngine.Image
---@field imageSpriteAddress string
---@field enemyName string
---@field spawnParent UnityEngine.Transform
---@field spawnPosition UnityEngine.Vector3
---@field spawnRotation UnityEngine.Quaternion
---@field spawnedObjects FreeVector
LoadTest = DefineClass()
{
    label = "",
    targetImage = nil,
    imageSpriteAddress = "",
    enemyName = "",
    spawnParent = nil,
    spawnPosition = UnityEngine.Vector3.zero,
    spawnRotation = UnityEngine.Quaternion.identity,
    spawnedObjects = FreeVector,
}
--初始化
function LoadTest:Init()
    self.spawnedObjects= FreeVector.S_NewFreeVector()
end
--用标签加载并实例化对象
---@param label string 标签
function LoadTest:LoadAndSpawnByLabel(label)
    FreeVector.S_NewFreeVector()
    local assets = ResourceManager:LoadAssetsByLabelAsync(label)
    for _, asset in ipairs(assets) do
        if(asset ~= nil) then
            local instance = ResourceManager:InstantiateAsync(asset.name, self.spawnPosition, self.spawnRotation)
            if(instance ~= nil) then
                if(self.spawnParent ~= nil) then
                    instance.transform:SetParent(self.spawnParent,false)
                end
                self.spawnedObjects:PushBack(instance)
            end
        end
    end
end
--加载并设置图片Sprite
---@param imageSpriteAddress string 图片Sprite地址
---@return Task 异步任务
function LoadTest:LoadAndSetSprite()
    local asset = ResourceManager:LoadAssetAsync(self.imageSpriteAddress)
    if(asset ~= nil) then
        self.targetImage.sprite = asset
        self.targetImage.preserveAspect = true
    end
end

--销毁实例释放所有资源
function LoadTest:Destroy()
    for i = 1, self.spawnedObjects:Size() do
        ResourceManager:DestroyInstance(self.spawnedObjects:At(i))
    end
    self.spawnedObjects:Clear()

    if(not string.IsNullOrEmpty(self.imageSpriteAddress)) then
        ResourceManager:ReleaseAssetsByLabel(self.imageSpriteAddress)
    end
    ResourceManager:ReleaseAssetsByLabel(self.label)
end