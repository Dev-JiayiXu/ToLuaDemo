---@class ResLoader 资源加载接口
ResLoader = DefineClass()

function ResLoader:New()
    local loader = New(self)
    
    return loader
end

function ResLoader:Destroy()
end

function ResLoader:Spawn()
end

