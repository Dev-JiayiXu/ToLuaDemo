---@class ResourceManager:IFrameworkMgr Lua侧封装的资源管理器
ResourceManager = DeriveClass(IFrameworkMgr) 

function ResourceManager:Init()
end
function ResourceManager:Reconnect()
end
function ResourceManager:Destroy()
end

---加载单个资源
---@param assetKey string 资源路径
---@return Task 异步任务
function ResourceManager:LoadAssetAsync(assetKey)
    return GameMain.resourceManager:LoadAssetAsync(assetKey)
end
---实例化对象
---@param assetKey string 资源路径
---@param position Vector3 位置
---@param rotation Quaternion 旋转
---@return Task 异步任务
function ResourceManager:InstantiateAsync(assetKey, position, rotation)
    return GameMain.resourceManager:InstantiateAsync(assetKey, position, rotation)
end

---销毁实例
---@param instance GameObject 要销毁的实例
function ResourceManager:DestroyInstance(instance)
    GameMain.resourceManager:DestroyInstance(instance)
end

---通过标签加载多个资源
function ResourceManager:LoadAssetsByLabelAsync(label)
    return GameMain.resourceManager:LoadAssetsByLabelAsync(label)
end

---释放标签资源
---@param label string 标签
function ResourceManager:ReleaseAssetsByLabel(label)
    GameMain.resourceManager:ReleaseAssetsByLabel(label)
end

---检查资源是否已加载
---@param assetKey string 资源路径
---@return boolean 是否已加载
function ResourceManager:IsAssetLoaded(assetKey)
    return GameMain.resourceManager:IsAssetLoaded(assetKey)
end

---获取资源引用计数
---@param assetKey string 资源路径
---@return number 引用计数
function ResourceManager:GetAssetRefCount(assetKey)
    return GameMain.resourceManager:GetAssetRefCount(assetKey)
end

return ResourceManager