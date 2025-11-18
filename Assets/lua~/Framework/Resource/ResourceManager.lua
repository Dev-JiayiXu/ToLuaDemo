---@class ResourceManager:IFrameworkMgr Lua侧封装的资源管理器
ResourceManager = DeriveClass(IFrameworkMgr) 

---@private
function ResourceManager:Init()
    self.resLoader = ResLoader:New()
end

---获取资源加载器
---@return ResLoader 资源加载器
function ResourceManager:GetResLoader()
    return self.resLoader
end

---预加载常用资源池
function ResourceManager:PreloadCommonPools()
    -- 预加载敌人池
    self.resLoader:CreatePoolForAsset("Enemy_01", 5, 20)
    self.resLoader:CreatePoolForAsset("Enemy_02", 5, 20)
    -- 添加其他常用资源池...
end

function ResourceManager:Destroy()
    if self.resLoader then
        self.resLoader:Destroy()
        self.resLoader = nil
    end
    self.resLoader = nil
end

return ResourceManager