[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/deanthecoder.svg?style=social&label=Follow%20%40deanthecoder)](https://twitter.com/deanthecoder)
# GLSL Shader Shrinker
Download from the [Releases](https://github.com/deanthecoder/GLSLShaderShrinker/releases) section.
## What Is It For?
GLSL Shader Shrinker is a Windows GUI tool that attempts to reduce the size of GLSL fragment shader code, whilst keeping it _readable_ and understandable.

It is written in C# using WPF and Visual Studio 2019, and has several hundred NUnit-powered unit tests.

![Main UI](img/ED209.png?raw=true "Main UI")

It is designed to work primarily with code from [Shadertoy](https://www.shadertoy.com/), but has limited support for other styles of GLSL too (E.g. [Bonzomatic](https://github.com/Gargaj/Bonzomatic))

After writing a Shadertoy shader, usually from my boilerplate starting code, there is a sequence of operations I perform:
* Delete dead/commented-out code.
* Remove unused functions.
* Inline some constants (Max raymarching distance, 'hit test' accuracy, ...)
* If trying to get under the magic '4KB', simplify some of the calculations.

It occurred to me all of these steps can be automated.

## What Is It *Not* For?
This is *not* a tool to compete with certain other tools to absolutely MINIMIZE the size of the code - Useful for preparing GLSL for use in 4KB graphics demos, etc.

GLSL Shader Shrink will _not_:
* Rename functions and variable to single-characters.
* Inline functions.
* Introduce ```#define``` macros to minimize the code character count.
* Replace code with a 'more compressible' equivalent.
* ...or otherwise '[GOLF](https://en.wikipedia.org/wiki/Code_golf)' anything.

## Example (Shadertoy Starting Point)
A small snippet of GLSL which shows **some** of the optimizations available.

### Before Processing
```glsl
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord/iResolution.xy;

    // Time varying pixel color
    vec3 col = 0.5 + 0.5*cos(iTime+uv.xyx+vec3(0,2,4));

    // Output to screen
    fragColor = vec4(col,1.0);
}
```
### After Processing
```glsl
void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 uv = fragCoord / iResolution.xy;
	fragColor = vec4((.5 + .5 * cos(iTime + uv.xyx + vec3(0, 2, 4))), 1);
}
```
* ```in``` parameter prefix removed.
* ```col``` variable inlined.
* Numbers simplified - Decimal places and in ```vec4``` construction.
* Comments removed.

**Note:** All these changes are optional within the tool, and many other optimizations are available.

---
## Getting Started

First, download the Windows installer from the 'Releases' section.

### Step 1 - Import GLSL Code
You first need to import your GLSL into the tool.

![Import](img/Import.png?raw=true "Import")

This can be achieved using:
* Copy'n'paste from the clipboard. (CTRL-V)
* Import from a text file.
* Download from [Shadertoy](https://www.shadertoy.com/) using an 'id'.

![Shadertoy](img/Shadertoy.png?raw=true "Shadertoy")

### Step 2 - Shrink GLSL Code
Next choose the level of processing you want to apply.

![Shrink](img/Shrink.png?raw=true "Shrink")

* Maximum processing - All options enabled.
* Minimal processing - Minimal changes (Mostly code reformatting).
* Custom options - Toggle exactly which processing features you require.

### Step 3 - Exporting GLSL Code
Export the 'shrunk' GLSL.

![Export](img/Export.png?raw=true "Export")

This can be achieved using:
* Copy'n'paste from the clipboard. (CTRL-C)
* Export from a text file.

...and then use with Shadertoy, Bonzomatic, etc.

---
## Limitations
Despite a lot of effort spent trying to ensure the tool produces great output every time, there are always going to be edge cases caused by different coding styles and patterns of content.

Heavy use of ```#define``` macros and ```#if...#else...#endif``` blocks can cause confusion when trying to parse the code.  Compilers have the luxury of seeing which specific code path is enabled, but a tool like this needs to understand **all** possible code paths at the same time - Not always easy!

I apologize in advance if you find any issues - If I have the time I'll try my best to resolve them!

In most cases they can be worked-around using a set of 'custom' settings which disable the problematic feature.

---
# Features
* [Remove Comments](#remove-comments)
* [Keep Header Comments](#keep-header-comments)
* [Remove Unused Functions](#remove-unused-functions)
* [Remove Unused Variables](#remove-unused-variables)
* [Remove Unreachable Code](#remove-unreachable-code)
* [Remove Disabled Code](#remove-disabled-code)
* [Simplify Function Declarations](#simplify-function-declarations)
* [Simplify Function Parameters](#simplify-function-parameters)
* [Group Variable Declarations](#group-variable-declarations)
* [Join Variable Declarations and Assignments](#join-variable-declarations-and-assignments)
* [Detect New Constants](#detect-new-constants)
* [Inline Constant Variables](#inline-constant-variables)
* [Inline Constant #defines](#inline-constant-#defines)
* [Simplify Number Format](#simplify-number-format)
* [Simplify Vector Construction](#simplify-vector-construction)
* [Simplify Vector References](#simplify-vector-references)
* [Simplify Code Branching](#simplify-code-branching)
* [Combine Consecutive Assignments](#combine-consecutive-assignments)
* [Combine Assignment With Single Use](#combine-assignment-with-single-use)
* [Introduce +=, -=, /=, *=](#introduce-+=,--=,-/=,-*=)
* [Simplify Mathematical Expressions](#simplify-mathematical-expressions)
* [Perform Simple Arithmetic](#perform-simple-arithmetic)
## Remove Comments
Remove all C/C++ -style comments from the code.
#### Before
```c
// This comment will be removed.
int myFunc(vec3 p) { return 1; }
```
#### After
```c
int myFunc(vec3 p) { return 1; }
```

---
## Keep Header Comments
Keep the top-most comments in the code, even when removing all others.
#### Before
```c
// 'My Shader' written Me.
// This comment will stay.
int aGlobal = 2;

// This comment will be removed.
int myFunc(vec3 p) { return 1; }
```
#### After
```c
// 'My Shader' written Me.
// This comment will stay.
int aGlobal = 2;

int myFunc(vec3 p) { return 1; }
```

---
## Remove Unused Functions
Remove any functions that are not called within the code.

**Note:** Only active if a `main...()` function is defined.

---
## Remove Unused Variables
Remove any global or local variables not used within the code.
#### Before
```c
int myFunc() {
    int unused = 2; // <-This will be removed.
    return 1;
}
```
#### After
```c
int myFunc(vec3 p) { return 1; }
```

---
## Remove Unreachable Code
Remove any code which cannot be reached.
#### Before
```c
float myFunc(vec3 p) {
    return p.x + p.y - p.z;

    // This code cannot be reached.
    a *= 2;
}
```
#### After
```c
float myFunc(vec3 p) {
    return p.x + p.y - p.z;
}
```

---
## Remove Disabled Code
Remove any commented-out code, or code surrounded with `#if 0...#endif`.
#### Before
```c
#if 1
float myFunc(vec3 p) { return p.x + p.y - p.z; }
#else
float myFunc(vec3 p) { return 3.141; }
#endif
```
#### After
```c
float myFunc(vec3 p) { return p.x + p.y - p.z; }
```

---
## Simplify Function Declarations
* Removes function declarations with no matching definition.
* Removes declarations where the matching definition is early enough to be used by all its callers.
* Removes declaration parameter names.

#### Before
```c
// Declare a function.
float sum(float value1, float value2);

// Define the function.
float sum(float value1, float value2) { return value1 + value2; }

// Use the function.
void main() { myFunc(1, 2); }
```
#### After
```c
// Define the function.
float sum(float value1, float value2) { return value1 + value2; }

// Use the function.
void main() { myFunc(1, 2); }
```

---
## Simplify Function Parameters
* Removes `void` parameters.
* Removes `in` keywords (which is the default in GLSL).

#### Before
```c
float myFunc(void) { return 3.141; }
float sum(in float a, in float b) { return a + b; }
```
#### After
```c
float myFunc() { return 3.141; }
float sum(float a, float b) { return a + b; }
```

---
## Group Variable Declarations
* Merge multiple declarations of the same variable type (when it makes sense to do so).
* Applies to global variables, local variables, and fields in a `struct`.

#### Before
```c
struct MyType {
    vec3 hit;
    vec3 color;
    vec2 uv;
};
```
#### After
```c
struct MyType {
    vec3 hit, color;
    vec2 uv;
};
```

---
## Join Variable Declarations and Assignments
Join variable declarations with their corresponding assignments, removing the need for the variable name to be specified twice.

#### Before
```c
float myFunc() {
    float result; // This will move.
    float b = 1.0;
    result = b * 3.141;
    return result;
}
```
#### After
```c
float myFunc() {
    float b = 1.0;
    float result = b * 3.141;
    return result;
}
```
**Note:** Fully simplified this would become...
```c
float myFunc() { return 3.141; }
```

---
## Detect New Constants
Find any variables assigned a value that can be made `const`.

**Note:** These can become candidates for inlining into the code, when used with other options.

#### Before
```c
float myFunc() {
    float PI = 3.141;
    return 2.0 * PI;
}
```
#### After
```c
float myFunc() {
    const float PI = 3.141;
    return 2.0 * PI;
}
```

---
## Inline Constant Variables
Remove a `const` variable by inlining it in all the places it is used.

**Note:** This will only be performed if it will result in shorter code.

#### Before
```c
const float MAX_DIST = 128.0;

bool isVisible(float dist) { return dist <= MAX_DIST; }
```
#### After
```c
bool isVisible(float dist) { return dist <= 128.0; }
```

---
## Inline Constant #defines
Remove a `#define` by inlining its (constant) value in all the places it is used.

**Note:** This will only be performed if it will result in shorter code.

#### Before
```c
#define MAX_DIST 128.0

bool isVisible(float dist) { return dist <= MAX_DIST; }
```
#### After
```c
bool isVisible(float dist) { return dist <= 128.0; }
```

---
## Simplify Number Format
Performs a variety of formatting changes to represent numbers using less characters.
#### Before
```c
float a = 1.200;
float b = 001.00;
float c = 23.0f;
float d = float(1.2);
float e = float(12);
float f = 123000.0;
int   g = int(1.2);
int   h = int(23);
```
#### After
```c
float a = 1.2;
float b = 1.;
float c = 23.;
float d = 1.2;
float e = 12.;
float f = 123e3;
int   g = 1;
int   h = 23;
```

---
## Simplify Vector Construction
Simplify the construction of vector and matrix types.
#### Before
```c
vec3 a = vec3(1.0, 2.0, 3.0);
vec2 b = vec2(4.0, 4.0);
vec3 c = a.xyz;
vec3 d = vec3(a);
```
#### After
```c
vec3 a = vec3(1, 2, 3);
vec2 b = vec2(4);
vec3 c = a;
vec3 d = a;
```

---
## Simplify Vector References
Simplify the construction of vector and matrix types.
#### Before
```c
vec3 a = vec3(1, 2, 3);
vec2 b = vec2(a.x, a.y);
vec3 c = vec2(a.x, a.y, a.z, a.x);
```
#### After
```c
vec3 a = vec3(1, 2, 3);
vec2 b = a.xy;
vec3 c = a.xyzx;
```

---
## Simplify Code Branching
Simplify branches by removing the ```else``` keyword where possible.
#### Before
```c
if (a == b)
    return a;
else // < Not required.
    return a + b;
```
#### After
```c
if (a == b)
    return a;
return a + b;
```

---
## Combine Consecutive Assignments
Consecutive assignments of the same variable can often be inlined.
#### Before
```c
float doMaths() {
    float a = myFunc();
    a = pow(a, 2.0);
    a = a + 23.3;
    return a;
}
```
#### After
```c
float doMaths() {
    float a = pow(myFunc(), 2.0) + 23.3;
    return a;
}
```

---
## Combine Assignment With Single Use
A variable assignment used on the next line can often be inlined, if that next line is an assignment or ```if``` condition.
#### Before
```c
float doMaths() {
    float a, b, c;
    a = myFunc();
    b = pow(a, 2.0);
    c = b * 2.2;
    return c;
}
```
#### After
```c
float doMaths() {
    float c;
    c = pow(myFunc(), 2.0) * 2.2;
    return c;
}
```
Also
#### Before
```c
bool f() {
    float a = getValue();
    if (a > 2.)
        return true;
    return false;
}
```
#### After
```c
bool f() {
    if (getValue() > 2.)
        return true;
    return false;
}
```

---
## Introduce +=, -=, /=, *=
Make use of a combined math operator/assignment when possible.
#### Before
```c
float doMaths() {
    float a = 2.1;
    a += 1.0;
    a = a * 3.141;
    return a;
}
```
#### After
```c
float doMaths() {
    float a = 2.1;
    a++;
    a *= 3.141;
    return a;
}
```

---
## Simplify Mathematical Expressions
Reduce unnecessary round brackets when performing arithmetic.
#### Before
```c
float doMaths() {
    return (2.0 * (3.141)) * (1.1 + 2.2);
}
```
#### After
```c
float doMaths() {
    return 2.0 * 3.141 * (1.1 + 2.2);
}
```

---
## Perform Simple Arithmetic
Pre-evaluate simple arithmetic.
E.g.
* Change ```a = b + -c``` to ```a = b - c```
* Change ```f * 1.0``` or ```f / 1.0``` to ```f```
* Change ```f + 0.0``` or ```f - 0.0``` to ```f```
* Remove ```f * 0.0``` (when safe).
* Change ```pow(3.0, 2.0)``` to ```9.0```
* Change ```float a = 1.2 / 2.3 * 4.5;``` to ```float a = 2.3478;```
* Change ```vec2 f = vec2(1.1, 2.2) + 3.3 * 4.4;``` to ```vec2 f = vec2(15.62, 16.72);```

---
