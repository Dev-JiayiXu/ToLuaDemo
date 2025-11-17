-- 基类工厂函数
---@class BaseClass
---@field name string 类名
---@field super BaseClass|nil 父类
---@field new fun(...):table 实例化方法
---@field ctor fun(instance: table, ...)|nil 构造函数
local BaseClass = function(classname, super)
    local class = {
        name = classname,
        super = super,
    }

    -- 设置继承关系
    if super ~= nil then
        setmetatable(class, { __index = super })
    end

    -- 实例化方法
    ---@param ... any 构造函数参数
    ---@return table 实例对象
    class.new = function(...)
        local instance = {
            class = class
        }
        setmetatable(instance, { __index = class })

        -- 递归调用构造函数
        ---@param instance table 实例对象
        ---@param cls BaseClass 当前类
        ---@param ... any 构造函数参数
        local function callConstructors(instance, cls, ...)
            if cls.super then
                callConstructors(instance, cls.super, ...)
            end
            if cls.ctor then
                cls.ctor(instance, ...)
            end
        end

        -- 调用构造函数
        callConstructors(instance, class, ...)
        return instance
    end
    
    return class
end

-- 基类A
---@class A
---@field level number 等级
---@field star number 星级
---@field ctor fun(level: number, star: number) 构造函数
---@field upgrade fun(self: A) 升级方法
A = BaseClass("A", nil)

---A构造函数
---@param level number 等级
---@param star number 星级
function A:ctor(level, star)
    print("A constructor called")
    self.level = level or 0
    self.star = star or 0
end

function A:upgrade()
    self.level = self.level + 1
    print("A upgraded to level " .. self.level)
end

-- 子类B继承自A
---@class B : A
---@field skill string 技能
---@field ctor fun(self: B, level: number, star: number, skill: string) 构造函数
---@field useSkill fun(self: B) 使用技能方法
B = BaseClass("B", A)

---B类构造函数
---@param level number 等级
---@param star number 星级
---@param skill string 技能
function B:ctor(level, star, skill)
    print("B constructor called")
    self.skill = skill or "none"
end

---使用技能方法
function B:useSkill()
    print("B using skill: " .. self.skill)
end

-- 测试单继承
local a = A:new(10, 5)
local b = B:new(30, 15, "fireball")
b:useSkill()

-- 多重继承实现：本质是查找index表
---创建多重继承类
---@param ... table 父类列表
---@return table 子类
function createClass(...)
    local parents = {...}
    local child = {
        parents = parents
    }

    -- 搜索函数
    ---@param key string 属性名
    ---@return any 属性值
    local function search(key)
        for i = 1, #parents do
            local value = parents[i][key]
            if value then
                return value
            end
        end
    end

    setmetatable(child, {
        __index = function(_, key)
            return search(key)
        end
    })

    -- 实例化方法
    ---@param ... any 构造函数参数
    ---@return table 实例对象
    function child:new(...)
        local instance = {
            class = child
        }
        
        setmetatable(instance, {
            __index = function(_, key)
                -- 先查找当前类
                local value = child[key]
                if value then return value end
                -- 再查找所有父类
                return search(key)
            end
        })

        -- 调用所有父类的构造函数
        for i = 1, #parents do
            if parents[i].ctor then
                parents[i].ctor(instance, ...)
            end
        end

        -- 调用子类自己的构造函数
        if child.ctor then
            child.ctor(instance, ...)
        end

        return instance
    end

    return child
end

-- C继承自A和B
---@class C : A, B
---@field magic number 魔法值
---@field ctor fun(self: C, level: number, star: number, skill: string, magic: number?) 构造函数
---@field castMagic fun(self: C, power: number?): number 施放魔法方法
C = createClass(A, B)

---C类构造函数
---@param level number 等级
---@param star number 星级
---@param skill string 技能
---@param magic number? 魔法值
function C:ctor(level, star, skill, magic)
    print("C constructor called")
    self.magic = magic or 100
end

---施放魔法方法
---@param power number? 魔法威力
---@return number 实际威力
function C:castMagic(power)
    print("C casting magic with power: " .. self.magic)
    return power
end

-- 多重继承测试
local c = C:new(50, 25, "lightning", 200)
c:upgrade()      -- 来自A的方法
c:useSkill()     -- 来自B的方法  
c:castMagic()    -- C自己的方法