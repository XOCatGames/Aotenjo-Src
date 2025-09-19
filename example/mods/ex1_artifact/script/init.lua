-- Dependent scripts needs to be included first
local util = require("util")
local artifact = require("artifact")

-- Mod Entry Function
function init()
    util.print("Hello World")
    artifact.register() -- Call functions in other scripts
end
