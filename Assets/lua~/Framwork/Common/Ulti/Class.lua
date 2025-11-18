---类定义
---@param anyTableOrNil table? 可以传入表或nil
---@return table 返回定义好的新类
function DefineClass(anyTableOrNil)
    anyTableOrNil = anyTableOrNil or {}
    anyTableOrNil.__index = anyTableOrNil
    return anyTableOrNil
end

---派生子类
---@param BaseClass table 基类
---@param SubClassOrNil table? 子类，可以填nil
---@return table 返回继承自BaseClass的子类
function DeriveClass(BaseClass, SubClassOrNil)
    SubClassOrNil = SubClassOrNil or {}
    setmetatable(SubClassOrNil, BaseClass)
    SubClassOrNil.__index = SubClassOrNil
    SubClassOrNil.__base = BaseClass
    return SubClassOrNil
end

---@desc 实例化出类的对象，如果有构造方法__ctor，则会调用构造方法
---@generic T:table
---@param Class T 用于实例化的类
---@return T Class的实例t
function New(Class, ...)
    local t = {}
    setmetatable(t, Class)
    if t.__ctor then
        t:__ctor(...)
    end
    return t
end