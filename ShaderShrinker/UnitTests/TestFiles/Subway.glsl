// 'Subway'
// Thanks to Evvvvil, Flopine, Nusan, BigWings, Iq and a bunch of others
// for sharing their knowledge!
//
// Comment-out 'MY_GPU_CAN_TAKE_IT' if your graphics card struggles.

#define MY_GPU_CAN_TAKE_IT  // Enable reflections.
#define ZERO min(iTime, 0.0)
#define MIN_DIST 0.0015

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

mat2 rot(float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(c, s, -s, c);
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdCylinder(vec3 p, float h, float r) {
  vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(h,r);
  return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
  vec3 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
  return length(pa - ba * h) - r;
}

float sdLink(vec3 p, float le, float r1, float r2) {
  vec3 q = vec3(p.x, max(abs(p.y) - le, 0.0), p.z);
  return length(vec2(length(q.xy) - r1, q.z)) - r2;
}

vec3 getRayDir(vec3 ro, vec3 lookAt, vec2 uv) {
    vec3 forward = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up = cross(forward, right);
    return normalize(forward + right * uv.x + up * uv.y);
}

vec2 min2(vec2 a, vec2 b) {
    return a.x < b.x ? a : b;
}

vec2 cappedMod(vec2 p, float c, vec2 l1, vec2 l2) {
    return p - c * clamp(round(p / c), -l1, l2);
}

vec3 mirrorX(vec3 p, float d) {
    p.x = abs(p.x) - d;
    return p;
}

vec3 repeatXZ(vec3 p, float c, vec2 l1, vec2 l2) {
    p.xz = cappedMod(p.xz, c, l1, l2);
    return p;
}

float sdTactileSlab(vec3 p) {
    p.z -= 0.4;
    
    // Main slab - ISO standard 40x40cm ! :)
    const float gap = 0.015;
    float d = sdBox(p, vec3(0.40 - gap, 0.05, 0.4 - gap));
    
    // Add the dimples.
    p.y -= 0.01;
    p.xz = cappedMod(p.xz + vec2(0.06666), 0.06666 * 2.0, vec2(2.), vec2(3.));
    return min(d, sdCylinder(p, 0.0225, 0.05));
}

float sdTactileSlabStrip(vec3 p) {
    p.xz = cappedMod(p.xz, 0.8, vec2(3.0, 0.0), vec2(4.0, 0.0));
    return sdTactileSlab(p);
}

float sdPavingSlab(vec3 p) {
    const float gap = 0.015;
    return sdBox(p, vec3(0.5 - gap, 0.05, 0.5 - gap));
}

float sdThinPavingSlab(vec3 p) {
    const float gap = 0.015;
    return sdBox(p - vec3(0.0, 0.0, 0.2), vec3(0.5 - gap, 0.05, 0.2 - gap));
}

const vec2 stepToStep = vec2(0.36, 0.66);
const float centerToWall = 0.4 * 7.0;
const float ceilingHeight = 6.0;

const vec3 lightPos1 = vec3(0.0, 4.5, 4.3);
const vec3 lightPos2 = vec3(0.0, 4.5, -0.6);

float glow = 0.0;
float flicker = 1.0;

#define WALL_ID          1.5
#define RAIL_ID          2.5
#define STAIR_STRIP_ID   3.5
#define MARBLE_ID        4.5
#define TACTILE_TILE_ID  5.5
#define FLOOR_TILE_ID    6.5

vec2 sdStep(vec3 p) {
    const float gap = 0.015;
    
    // Top edge.
    float d1 = sdBox(p, vec3(1.4, 0.02, 0.02));
   
    if (d1 > 1.0)
        return vec2(d1, STAIR_STRIP_ID); // Too far away from the step to bother rendering further.
    
    // Front facing surface.
    p.y += 0.16 + 0.02;
    float d2 = sdBox(p, vec3(1.4 - gap, 0.16 - gap, 0.02));
    
    // Vertical grout.
    d1 = min(d1, sdBox(p - vec3(0.0, 0.0, gap), vec3(1.4, 0.16, 0.02)));
    
    // Horizontal surface.
    p.yz += vec2(0.16 + 0.02, 0.32);
    d2 = min(d2, sdBox(p, vec3(1.4 - gap, 0.02, 0.32 - gap)));

    // Horizontal grout.
    d1 = min(d1, sdBox(p + vec3(0.0, gap, 0.0), vec3(1.4, 0.02, 0.32 - gap)));
    
    return min2(vec2(d1, STAIR_STRIP_ID), vec2(d2, MARBLE_ID));
}

vec2 sdSteps(vec3 p) {
    float i = max(0.0, floor(-p.z / stepToStep.y));
    return sdStep(p + i * vec3(0.0, stepToStep));
}

float sdRailHolder(vec3 p) {
    p.x -= 0.35;
    return max(sdLink(p.yxz, 0.1, 0.25, 0.06), p.y);
}

vec2 sdWalls(vec3 p) {
    const float gap = 0.015;
    
    vec3 op = p;

    // Wall tiles.
    p.yz = mod(p.yz, vec2(0.16, 0.22));
    float d = sdBox(mirrorX(p, centerToWall), vec3(0.02, 0.16 - gap, 0.22 - gap)) - gap;

    // Hand rails.
    vec3 pp = mirrorX(op, centerToWall);
    pp.xy -= vec2(-0.3, 1.3);
    float d2 = 1e10;
    
    if (pp.y < 3.0) { // Skip stair rails if point is too far away.
        d2 = sdCapsule(pp, vec3(0.0, 0.0, 1.0), vec3(0.0), 0.1);
        d2 = smin(d2, sdRailHolder(pp - vec3(0.0, 0.0, 0.75)), 0.025);
        pp.yz *= rot(-atan(stepToStep.x / stepToStep.y));
        d2 = smin(d2, sdRailHolder(pp - vec3(0.0, 0.0, -0.75)), 0.0255);
        d2 = min(d2, sdCapsule(pp, vec3(0.0), vec3(0.0, 0.0, -length(stepToStep * 11.0)), 0.1));
        d2 = smin(d2, sdRailHolder(pp - vec3(0.0, 0.0, -length(stepToStep * 11.0) + 0.75)), 0.025);
    }

    if (p.x > 0.0) {
        // Remove the parts of the right wall.
        const float nearEndZ = -stepToStep.y * 12.0;
        float farEndZ = 2.9;
        float middleZ = mix(nearEndZ, farEndZ, 0.5);
        float depthToInclude = farEndZ - nearEndZ;
        pp = op;
        pp.z -= middleZ;
        d = max(d, sdBox(pp, vec3(3.0, 1000.0, depthToInclude / 2.0)));
    }
    
    return min2(vec2(d, WALL_ID), vec2(d2, RAIL_ID));
}

vec2 sdCorridorSection(vec3 p) {
    // Walls.
    vec2 d1 = sdWalls(p);
    if (p.y > 2.0)
        return d1; // Too far away from the floor to bother rendering further.
    
    // Steps.
    vec2 d2 = sdSteps(repeatXZ(p, 2.8, vec2(1.0, 0.0), vec2(1.0, 0.0)));

    // Pre-stairs half strip.
    float d = sdThinPavingSlab(repeatXZ(p, 1.0, vec2(3.0, 0.0), vec2(3.0, 0.0)));

    // Tactile slabs.
    p.z -= 0.4;
    float d3 = sdTactileSlabStrip(p);
    d3 -= 0.006 * mix(0.8, 1.0, texture(iChannel0, p.xz * 1.7).r);

    // Floor.
    d = min(d, sdPavingSlab(repeatXZ(p - vec3(0.0, 0.0, 0.8 + 0.5), 1.0, vec2(3.0, 0.0), vec2(3.0, 5.0))));
    
    return min2(vec2(d, FLOOR_TILE_ID), min2(d1, min2(d2, vec2(d3, TACTILE_TILE_ID))));
}

vec2 map(vec3 p, bool useGlow) {
    // Side wire cover.
    float d = sdBox(p - vec3(centerToWall - 0.1, 0.0, 1.8), vec3(0.07, 10.0, 0.07)) - 0.01;

    // Overhead beam.
    d = min(d, sdBox(p - vec3(0.0, ceilingHeight - 1.0, 1.8), vec3(centerToWall, 1.0, 0.07)) - 0.01);
    
    // Overhead pipe.
    vec3 v = vec3(centerToWall - 1.0, ceilingHeight - 1.5, 0.0);
    d = min(d, sdCapsule(p, v + vec3(0.0, 0.0, 2.9), v - vec3(0.0, 0.0, 100.0), 0.18));
    d = min(d, sdCapsule(p, v + vec3(0.0, 0.0, 2.0), v + vec3(0.0, 0.0, 1.6), 0.22));
    
    // Overhead pipe clamps.
    vec3 pp = p;
    pp.z = abs(p.z - 1.8);
    d = min(d, sdLink(pp - vec3(centerToWall - 1.0, ceilingHeight - 1.3, 0.9), 0.2, 0.2, 0.015));
    d = min(d, sdLink(p - vec3(centerToWall - 1.0, ceilingHeight - 1.3, -2.0), 0.2, 0.2, 0.015));

    // Ceiling lights.
    pp -= vec3(0.0, ceilingHeight - 1.0, 2.5);
    float lightFrame = max(sdBox(pp, vec3(1.0, 0.25, 0.06)), -sdBox(pp + vec3(0.0, 0.3, 0.0), vec3(0.95, 0.2, 0.1)));
    d = min(d, lightFrame);
    
    // Only apply glow when marching, not calculating normals.
    if (useGlow) {
        float endFade = 1.0 - clamp((abs(p.x) - 0.8) / 0.2, 0.0, 1.0);
        pp.y += 0.2;
        float gd = sdCylinder(pp.yxz, 0.05, 0.92);
        glow += endFade * 0.001 / (0.001 + gd * gd * 0.3) * mix(0.01, 1.0, p.z < 0.0 ? 1.0 : flicker);
    }
    
    // Ceiling.
    pp = p;
    pp.xz *= rot(-3.1415 / 4.0);
    float bump = texture(iChannel0, p.xz * 0.8).r * 0.01;
    d = min(d, sdBox(pp - vec3(0.0, ceilingHeight, -20.0), vec3(10.0, 1.0, 20.0 + stepToStep * 12.0)) - bump);

    // Base corridor.
    vec2 d2 = sdCorridorSection(p);
    
    // Upper corridor.
    p.yz -= stepToStep * 12.0;
    p.z -= 9.0;
    p.xz *= rot(-3.14159 / 4.0);
    p.x -= 9.08;
    
    d2 = min2(d2, sdCorridorSection(p));
    
    return min2(vec2(d, WALL_ID), d2);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(1.0, -1.0) * 0.5773 * 0.0001;
    return normalize(e.xyy * map(p + e.xyy, false).x + 
					 e.yyx * map(p + e.yyx, false).x + 
					 e.yxy * map(p + e.yxy, false).x + 
					 e.xxx * map(p + e.xxx, false).x);
}

float calcAO(vec3 p, vec3 n, float d) {
    return clamp(map(p + n * d, false).x / d, 0.0, 1.0);
}

float calcShadow(vec3 p, vec3 lightPos) {
    float d = distance(p, lightPos);
    
    float shadow = 1.0;
    vec3 st = (lightPos - p) / 30.0 * 0.8;
    float std = length(st);
    p += normalize(lightPos - p) * 0.01;
    for (float i = ZERO; i < 30.0; i++) {
        p += st;
        shadow = min(shadow, max(map(p, false).x, 0.0) / (std * i));
    }
    
    float falloff = pow(d / 20.0 + 1.0, 2.0);
    return shadow / falloff;
}

/**********************************************************************************/

vec3 vignette(vec3 col, vec2 fragCoord) {
    vec2 q = fragCoord.xy / iResolution.xy;
    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.4);
    return col;
}

vec3 getMaterial(vec3 p, vec3 rd, vec3 n, float id) {
    vec3 lightDir1 = normalize(lightPos1 - p);
    vec3 lightDir2 = normalize(lightPos2 - p);
    vec3 lightCol = vec3(1.0, 1.0, 1.1);

    float spec = pow(
        max(
            max(
                dot(reflect(lightDir1, n), rd) * flicker,
                dot(reflect(lightDir2, n), rd)
            ),
            0.0
        ),
        50.0);

    vec3 mat;
    if (id == STAIR_STRIP_ID) {
        mat = vec3(0.1);
    } else if (id == MARBLE_ID) {
        mat = vec3(smoothstep(0.0, 0.6, texture(iChannel0, (abs(n.y) < 0.1 ? p.xy : p.xz) * 1.4125).r));
    } else if (id == TACTILE_TILE_ID) {
        mat = vec3(0.9, 0.75, 0.21); // Yellow.
        spec *= 0.8; // Reduce specular contribution.
    } else if (id == FLOOR_TILE_ID) {
        mat = vec3(mix(0.3, 0.5, texture(iChannel0, p.xz * 1.743).r));
    } else mat = vec3(1.0); // White (Tiles, etc)

    // Diffuse color.
    float diff = max(max(0.0, dot(lightDir1, n) * flicker), dot(lightDir2, n));

    // Fake ambient occlusion.
    float occ = min(1.0, 0.2 + calcAO(p, n, 0.15) * calcAO(p, n, 0.05));

    // Shadows from two light sources.
    float sha = (calcShadow(p, lightPos1) * flicker + calcShadow(p, lightPos2)) / 2.0;

    vec3 col = mat * lightCol * ((diff + spec) * sha + occ * 0.025);
    
    // Global 'glow' variable accumulates color as the
    // scene is marched.
    return col + min(glow, 1.0);
}

void march(vec3 ro, vec3 rd, out vec3 p, out vec2 h) {
    float d = 0.01;
    for (float steps = ZERO; steps < 60.0; steps++) {
        p = ro + rd * d;
        h = map(p, true);
        
        if (abs(h.x) < MIN_DIST * d) break; // We've hit a surface - Stop.
        d += h.x * 0.9; // No hit, so keep marching.
    }
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord - 0.5 * iResolution.xy) / iResolution.y;
    
    flicker = step(0.25, sin(iTime) * texture(iChannel0, vec2(iTime * 0.1)).r);

    // Camera.
    float ft = fract(iTime / 5.0);
    float phase = mod(floor(iTime / 5.0), 3.0);
    
    vec3 ro;
    vec3 lookAt = vec3(0.0, 1.0, 0.0);
    
    if (phase == 0.0) {
        ro = vec3(mix(0.0, -0.5, ft) * -3.0 - 1.0,
                  -1.0 + -6.0 * mix(0.5, 0.4, ft),
                  -10.0);
    } else if (phase == 1.0) {
        ro = vec3(-3.0 * mix(0.0, 0.5, ft) - 1.0, 3.0, -4.0);
        lookAt = lightPos2;
    } else if (phase == 2.0) {
        ro = vec3(0.5, -1.0 + -6.0 * (mix(0.25, 0.0, ft) - 0.5), -1.0);
        lookAt = lightPos1 - mix(vec3(5.0, 3.5, -1.0), vec3(0.0), ft);
    }
    
    vec3 rd = getRayDir(ro, lookAt, uv);
    
    // Raymarch.
    vec2 h;
    vec3 p;
	march(ro, rd, p, h);

    // Materials and lighting.
	vec3 n = calcNormal(p);
    vec3 col = getMaterial(p, rd, n, h.y);

#ifdef MY_GPU_CAN_TAKE_IT
    // Reflect on hand rails.
    if (h.y == RAIL_ID) {
        rd = reflect(rd, n);
        march(p, rd, p, h);
        col = mix(col, getMaterial(p, rd, n, h.y), 0.75);
    }
#endif
    
    // Fog.
    col *= exp(-pow(distance(ro, p) / 30.0, 3.0) * 5.0);
    
    // Output to screen.
    col = vignette(pow(col, vec3(0.4545)), fragCoord);
    fragColor = vec4(col, 1.0);
}