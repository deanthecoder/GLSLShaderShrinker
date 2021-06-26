// https://www.shadertoy.com/view/3tXczn
#define FAST_NOISE
//#define USE_WEBCAM

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(123.45, 87.43))) * 5432.3);
}

float noise(vec2 p) {
#ifdef FAST_NOISE
    p *= 0.05;
    return texture(iChannel1, p).r;
#else
    vec2 i = floor(p);
    vec2 f = fract(p);
    
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    
    return mix(a, b, f.x) +
            (c - a) * f.y * (1.0 - f.x) +
            (d - b) * f.x * f.y;
#endif
}

float fbm(vec2 p) {
	float f;
    f  = 0.5 * noise(p * 1.1);
    f += 0.22 * noise(p * 2.3);
    f += 0.0625 * noise(p * 8.4);
    return f / 0.7825;
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

mat2 rot(float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(c, s, -s, c);
}

vec2 min2(vec2 a, vec2 b) {
    return a.x < b.x ? a : b;
}

float sdTorus(vec3 p, vec2 t) {
  return length(vec2(length(p.xy) - t.x, p.z)) - t.y;
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdCorridorRibs(vec3 p) {
    float d = 0.6;
    p.z = mod(p.z, d) - d * 0.5;
    
    return sdTorus(p, vec2(2.0, 0.2));
}

float sdCorridorTube(vec3 p) {
    
    float lxy = length(p.xy);
    float d = max(lxy - 2.0, 1.9 - lxy);
    
    float t = fbm((p.xy + p.yz) * 4.0) * 0.05;
    return d - t;
}

float sdCorridorRoof(vec3 p) {
    p.y -= 2.5;
    float rib = abs(0.5 + 0.5 * sin(p.z * 1.4));
    return length(p.xy) - 1.2 + 0.2 * pow(rib, 4.0);
}

vec3 applyCorriderCurve(vec3 p) {
    // Curve the tunnel (Remember there is no spoon!)
    p.xz *= rot(-0.01 * p.z);
    
    // Walk.
    if (iTime < 44.0)
    	p.z += iTime;
    else
        p.z += 44.0 + 2.6 * smoothstep(0.0, 1.0, min(1.0, (iTime - 44.0) / 2.6));
    
    return p;
}

#define EGG_Z 49.0

float sdCorridorFloor(vec3 p) {
    p = applyCorriderCurve(p);
    p.y += sin(p.x * 1.1 + sin(p.z * 0.7)) * 0.15;
    float d = p.y + 1.5 + sin(p.z) * 0.05;
    
    d = min(d, length(p - vec3(0.0, -3.3, EGG_Z)) - 2.0);
    
    return d - fbm(p.xz) * 0.1;
}

float sdCorridor(vec3 p) {
    p = applyCorriderCurve(p);
    vec3 pp = p;
    pp.x = abs(pp.x) - 0.8;
    float d = smin(sdCorridorRibs(pp), sdCorridorTube(pp), 0.1);
    d = smin(d, sdCorridorRoof(p), 0.3);
    
    return d;
}

float sdEgg(vec3 p) {
    p = applyCorriderCurve(p);
    
    vec3 pp = p;
    pp.z -= EGG_Z;
    pp.y += 1.0;
    
    float d;

    pp.y *= 0.7;
    d = length(pp) - 0.4;
    
    float startOpenTime = 45.0;
    float openness = min(1.0, max(0.0, iTime - startOpenTime) * 0.1);
    if (iTime >= 55.0)
        openness += sin((iTime - 55.0) * 1.0) * 0.05;
    
    float cutOut = length(pp.xz) - p.y * 0.5 - mix(0.1, 0.7, openness);
    d = smin(d, -cutOut, -0.1);
    
    float rim = sdTorus(
        (pp - vec3(0.0, mix(0.40, 0.25, openness), 0.0)).xzy,
        vec2(0.35, 0.04) * openness);
    d = smin(d, rim, 0.05 * openness);
    
    d -= fbm(pp.xz + pp.xy) * 0.05;
    
    return d;
}

float sdSurprise(vec3 p) {
    float t = min(1.0, max(0.0, iTime - 62.0));
    if (t <= 0.0) return 1e10;

    p = applyCorriderCurve(p);
    p.z -= EGG_Z;
    
    p.y += 1.5 - 1.4 * sin(t * 3.14159 * 0.5);
    
    float tt = max(0.0, iTime - 62.8) * 2.5;
    return length(p) - mix(0.1, 2.0, clamp(0.0, 1.0, tt));
}

vec3 getRayDir(vec3 ro, vec3 lookAt, vec2 uv) {
    vec3 forward = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up = cross(forward, right);
    return normalize(forward + right * uv.x + up * uv.y);
}

vec2 map(vec3 p) {
    vec2 d1 = vec2(sdCorridor(p), 1.5);
    vec2 d2 = vec2(sdCorridorFloor(p), 2.5);
    vec2 d3 = vec2(sdEgg(p), 3.5);
    
    vec2 d = min2(min2(d1, d2), d3);
    d = min2(d, vec2(sdSurprise(p), 4.5));
    return d;
}

vec3 calcNormal(in vec3 p) {
    // Thanks iq!
    vec2 e = vec2(1.0, -1.0) * 0.5773 * 0.0005;
    return normalize(e.xyy * map(p + e.xyy).x + 
					 e.yyx * map(p + e.yyx).x + 
					 e.yxy * map(p + e.yxy).x + 
					 e.xxx * map(p + e.xxx).x);
}

float calcShadow(vec3 p, vec3 lightPos, float sharpness) {
    vec3 rd = normalize(lightPos - p);
    
    float h;
    float minH = 1.0;
    float d = 0.01;
    for (int i = 0; i < 16; i++) {
        h = map(p + rd * d).x;
        minH = abs(h / d);
        if (minH < 0.01)
            return 0.0;
        d += h;
    }
    
    return minH * sharpness;
}

float calcOcc(vec3 p, vec3 n, float strength) {
    const float dist = 0.3;
    return 1.0 - (dist - map(p + n * dist).x) * strength;
}

float calcSpotlight(vec3 p, vec3 lightPos, vec3 lightDir, float cutOff, float edgeBlur) {
    float l = dot(normalize(lightPos - p), -lightDir);
    edgeBlur += 1.0;
    float spotLight = smoothstep(1.0 - cutOff, (1.0 - cutOff) * edgeBlur, l) * 0.3;
    cutOff *= 0.7;
    spotLight = max(spotLight, smoothstep(1.0 - cutOff, (1.0 - cutOff) * 1.06, l)) * 0.5;
    cutOff *= 0.7;
    return max(spotLight, smoothstep(1.0 - cutOff, (1.0 - cutOff) * 1.07, l));
}

vec3 vignette(vec3 col, vec2 fragCoord) {
    vec2 q = fragCoord.xy / iResolution.xy;
    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.4);
    return col;
}

vec2 getTorchDir() {
    
    float tSeg = iTime;
    
    // Look at ceiling.
    if (tSeg < 2.0) return vec2(0.0, 1.0);
    tSeg -= 2.0;
    
    // Look ahead.
    if (tSeg < 4.0) return vec2(0.0, mix(1.0, 0.0, smoothstep(0.0, 1.0, min(1.0, tSeg / 2.0))));
    tSeg -= 4.0;
    
    // Down to floor.
    if (tSeg < 3.5) return vec2(0.0, mix(0.0, -0.4, smoothstep(0.0, 1.0, min(1.0, tSeg / 1.5))));
    tSeg -= 3.5;

    // Up the wall.
    if (tSeg < 4.0) {
        float f = smoothstep(0.0, 1.0, min(1.0, tSeg / 4.0));
        return vec2(sin(f * 3.141) * -0.6, -0.4 + 1.1 * sin(f * 3.141 / 2.0));
    }
    tSeg -= 4.0;

    // Look ahead - Walking.
    if (tSeg < 12.0) return vec2(0.0, mix(0.7, -0.2, smoothstep(0.0, 1.0, min(1.0, tSeg))));
    tSeg -= 12.0;

    // Is that an egg?
    if (tSeg < 17.0) return vec2(0.0, mix(-0.2, -0.05, smoothstep(0.0, 1.0, min(1.0, tSeg / 5.0))));
    tSeg -= 17.0;

    
    // Let's have a look...
    return vec2(0.0, mix(-0.05, -0.35, smoothstep(0.0, 1.0, min(1.0, tSeg / 5.0))));
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord - 0.5 * iResolution.xy) / iResolution.y;

#ifdef USE_WEBCAM
    if (iTime > 63.5) {
    	fragColor = vec4(mix(vec3(0.0), texture(iChannel0, fragCoord / iResolution.xy).rgb, min(1.0, (iTime - 63.5) * 5.0)), 1.0);
        return;
    }
#endif

    // Raymarch.
    vec3 torchDir = normalize(vec3(getTorchDir(), 0.8));
    
    vec2 walkBump = vec2(sin(iTime * 2.5) * 0.05, pow(0.5 + 0.5 * sin(iTime * 5.0), 2.0) * 0.03);
    walkBump *= mix(1.0, 0.0, min(1.0, max(0.0, iTime - 44.0) / 2.6));
    
    vec3 ro = vec3(walkBump, 0.0);
    vec3 rd = getRayDir(ro, torchDir + vec3(0.0, 0.1, 0.0), uv);

    int hit = 0;
    float d = 0.01;
    vec3 p;
    for (float steps = 0.0; steps < 120.0; steps++) {
        p = ro + rd * d;
        vec2 h = map(p);
        if (h.x < 0.005 * d) {
            hit = int(h.y);
            break;
        }

        d += h.x;
    }

    vec3 col;
    if (hit > 0) {
        vec3 n = calcNormal(p);
        vec3 lightPos = vec3(0.0, -0.75, 0.0);
        vec3 lightCol = vec3(1.0, 0.9, 0.8);
        vec3 lightToPoint = normalize(lightPos - p);
        vec3 skyCol = vec3(0.15, 0.2, 0.25);
        float sha = calcShadow(p, lightPos, 5.0);
        float occ = calcOcc(p, n, 4.0);
        float spe = pow(max(0.0, dot(rd, reflect(lightToPoint, n))), 3.0);
        float torch = calcSpotlight(p, lightPos, torchDir, 0.1, 0.02);
        float backLight = clamp(dot(n, -rd), 0.01, 1.0) * 0.05;
        float fog = 1.0 - exp(-d * 0.006);

        vec3 mat;
        if (hit == 1) {
            // Tunnel walls.
            mat = vec3(0.05, 0.06, 0.05);
        } else if (hit == 2) {
            // Tunnel floor.
            mat = vec3(0.055, 0.06, 0.06) * 0.6;
        } else if (hit == 3) {
            // Egg.
            mat = mix(vec3(0.5, 0.3, 0.2), vec3(0.05, 0.06, 0.05), 0.7);
        } else if (hit == 4) {
            // Surprise.
            mat = vec3(0.0);
        }

        col = (torch * sha + (backLight + spe) * occ) * lightCol;
        col *= mat;
        col += torch * 0.02 * lightCol;
        col = mix(col, skyCol, fog);
    }

    // Output to screen
    col = pow(vignette(col, fragCoord), vec3(0.4545));
    fragColor = vec4(col, 1.0);
}