require("Requires")

---@class luaEntry
---@field  _frameworks IFrameworkMgr[]
luaEntry = DefineClass{
    _frameworks = {}
}

function luaEntry:Init()
    self:AddFrameworks()
    for i, framework in ipairs(self._frameworks) do
        if framework.Init then
            framework:Init()
        end
    end
end
function luaEntry:AddFrameworks()
    self:AddFramework(ResourceManager)
    self:AddFramework(BagProxy)
end

---@generic T: IFrameworkMgr
---@param framework T
---@return T
function luaEntry:AddFramework(framework)
    self._frameworks[#self._frameworks + 1] = framework
    return framework
end
function luaEntry:Destroy()
    for i = #self._frameworks, 1, -1 do
        self._frameworks[i]:Destroy()
    end
end

luaEntry:Init()

return luaEntry