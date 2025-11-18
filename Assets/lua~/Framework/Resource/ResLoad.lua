---@class ResLoader 资源加载接口
ResLoader = DefineClass()

function ResLoader:New()
    local loader = New(self)
    return loader
end

---异步加载资源
---@param assetKey string 资源路径
---@return any 加载的资源
function ResLoader:LoadAssetAsync(assetKey)
    local task = GameMain.resourceManager:LoadAssetAsync(assetKey)
    return self:AwaitTask(task)
end

---实例化对象
---@param assetKey string 资源路径
---@param position Vector3 位置
---@param rotation Quaternion 旋转
---@return GameObject 实例化的对象
function ResLoader:InstantiateAsync(assetKey, position, rotation)
    local task = GameMain.resourceManager:InstantiateAsync(assetKey, position, rotation)
    return self:AwaitTask(task)
end

---销毁实例
---@param instance GameObject 要销毁的实例
function ResLoader:DestroyInstance(instance)
    GameMain.resourceManager:DestroyInstance(instance)
end

---通过标签加载多个资源
---@param label string 标签
---@return table 资源列表
function ResLoader:LoadAssetsByLabelAsync(label)
    local task = GameMain.resourceManager:LoadAssetsByLabelAsync(label)
    return self:AwaitTask(task)
end

---释放标签资源
---@param label string 标签
function ResLoader:ReleaseAssetsByLabel(label)
    GameMain.resourceManager:ReleaseAssetsByLabel(label)
end

---检查资源是否已加载
---@param assetKey string 资源路径
---@return boolean 是否已加载
function ResLoader:IsAssetLoaded(assetKey)
    return GameMain.resourceManager:IsAssetLoaded(assetKey)
end

---获取资源引用计数
---@param assetKey string 资源路径
---@return number 引用计数
function ResLoader:GetAssetRefCount(assetKey)
    return GameMain.resourceManager:GetAssetRefCount(assetKey)
end

---获取对象池状态
---@param poolName string 池名称
---@return number, number 可用数量, 总数量
function ResLoader:GetPoolStatus(poolName)
    return GameMain.resourceManager:GetPoolStatus(poolName)
end

---等待异步任务完成
---@param task any 异步任务
---@return any 任务结果
function ResLoader:AwaitTask(task)
    while not task:get_IsCompleted() do
        coroutine.yield()
    end
    return task.Result
end

function ResLoader:Destroy()
    self:Clear()
end

return ResLoader