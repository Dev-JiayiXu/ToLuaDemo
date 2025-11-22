---@class LoadTest
LoadTest = DefineClass()

function LoadTest:Start()
    if not GameMain then
        print("GameMain为空")
    else
        print("GameMain不为空")
    end

    self:LoadAndSpawnByLabel()
    self:LoadAndSetSprite()
    self:LoadAndSpawnByName()
end
--用标签加载一组物体
function LoadTest:LoadAndSpawnByLabel()
    if not GameMain.resourceManager then
        print("警告: GameMain.resourceManager 不存在，尝试直接获取")
    end

    local assets = ResourceManager:LoadAssetsByLabelAsync(self.label)
    for _, asset in ipairs(assets) do
        if asset ~= nil then
            local instance = ResourceManager:Instantiate(asset.name, self.spawnPosition, self.spawnRotation)
            if instance ~= nil and self.spawnParent ~= nil then
                instance.transform:SetParent(self.spawnParent, false)
            end
        end
    end
end

function LoadTest:LoadAndSpawnByName()
    if string.IsNullOrEmpty(self.enemyName) then 
        return 
    end
    
    local enemy = ResourceManager:InstantiateAsync(self.enemyName, self.spawnPosition, self.spawnRotation)
    if enemy ~= nil and self.spawnParent ~= nil then
        enemy.transform:SetParent(self.spawnParent, false)
    end
end

function LoadTest:LoadAndSetSprite()
    if string.IsNullOrEmpty(self.imageSpriteAddress) or self.targetImage == nil then return end
    
    local sprite = ResourceManager:LoadAssetAsync(self.imageSpriteAddress)
    if sprite ~= nil then
        self.targetImage.sprite = sprite
        self.targetImage.preserveAspect = true
        print("图片加载成功!")
    else
        print("图片加载失败!")
    end
end

function LoadTest:OnDestroy()
    if not string.IsNullOrEmpty(self.imageSpriteAddress) then
        ResourceManager:ReleaseAsset(self.imageSpriteAddress)
    end
    
    if not string.IsNullOrEmpty(self.label) then
        ResourceManager:ReleaseAssetsByLabel(self.label)
    end
end

return LoadTest