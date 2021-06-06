# GLSL Shader Shrinker
Optimizes the size of GLSL shader code.

# Features
### Remove Comments
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
### Keep Header Comments
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
### Remove Unused Functions
Remove any functions that are not called within the code.

**Note:** Only active if a `main...()` function is defined.

---
### Remove Unused Variables
Remove any global or local variables not used within the code.
#### Before
```c
int myFunc() {
int unused = 2; // &lt;-This will be removed.
return 1;
}
```
#### After
```c
int myFunc(vec3 p) { return 1; }
```

---
### Remove Unreachable Code
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
### Remove Disabled Code
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
### Simplify Function Declarations
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
### Simplify Function Parameters
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
### Group Variable Declarations
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
### Join Variable Declarations and Assignments
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
### Detect New Constants
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
### Inline Constant Variables
Remove a `const` variable by inlining it in all the places it is used.

**Note:** This will only be performed if it will result in shorter code.

#### Before
```c
const float MAX_DIST = 128.0;

bool isVisible(float dist) { return dist &lt;= MAX_DIST; }
```
#### After
```c
bool isVisible(float dist) { return dist &lt;= 128.0; }
```

---
### Inline Constant #defines
Remove a `#define` by inlining its (constant) value in all the places it is used.

**Note:** This will only be performed if it will result in shorter code.

#### Before
```c
#define MAX_DIST 128.0

bool isVisible(float dist) { return dist &lt;= MAX_DIST; }
```
#### After
```c
bool isVisible(float dist) { return dist &lt;= 128.0; }
```

---
### Simplify Float Number Format
Performs a variety of formatting changes to represent numbers using less characters.
#### Before
```c
float a = 1.200;
float b = 1.00;
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
### Simplify Vector Construction
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
### Simplify Vector References
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
### Simplify Code Branching
Simplify branches by removing the ```else``` keyword where possible.
#### Before
```c
if (a == b)
return a;
else // &lt; Not required.
return a + b;
```
#### After
```c
if (a == b)
return a;
return a + b;
```

---
### Combine Consecutive Assignments
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
### Combine Assignment With Single Use
An assignment used on the next line can often be inlined.
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

---
### Introduce +=, -=, /=, *=
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
### Simplify Mathematical Expressions
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
### Perform Simple Arithmetic
Pre-evaluate simple arithmetic.
E.g.
* Change ```a = b + -c``` to ```a = b - c```
* Change ```f * 1.0``` or ```f / 1.0``` to ```f```
* Change ```f + 0.0``` or ```f - 0.0``` to ```f```
* Remove ```f * 0.0``` (when safe).
* Change ```pow(3.0, 2.0)``` to ```9.0```
* Change ```float a = 1.2 / 2.3 * 4.5``` to ```a = 2.3478```

---
