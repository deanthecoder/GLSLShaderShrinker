// https://www.shadertoy.com/view/WtfyWj
// 'Ocean Treasure'
// Thanks to Evvvvil, Flopine, Nusan, BigWings, and a bunch of others for sharing their knowledge!

#define time (iTime + 37.0)

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

mat2 rot(float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(c, s, -s, c);
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

float sdBox(vec3 p, vec3 b) {
  vec3 q = abs(p) - b;
  return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdRod(vec3 p, float h, float r) {
  p.y -= clamp( p.y, 0.0, h );
  return length( p ) - r;
}

float sdSurface(vec2 p) {
    float sh = texture(iChannel0, (p + vec2(0.0, 1.0) * (time + 1.0)) * 0.05).r;
    sh -= texture(iChannel0, (p + vec2(0.7, 0.2) * time) * 0.05).r;
    return clamp(0.05 + sh * 0.2, 0.0, 1.0);
}

float sdChest(vec3 p) {
    if (length(p) > 4.5) return 1e7; // Ray not close enough to bother rendering.
    
    float w = 1.0;
    float l = 1.5;
    float h = 0.6;
    
    vec3 pp = p + vec3(0.0, h, 0.0);
    vec3 bs = vec3(w, h, l);
    float box = sdBox(pp , bs);
    float boxInner = sdBox(pp - vec3(0.0, 0.9, 0.0), bs);
    box = max(box, -boxInner);

    p.xy *= rot(0.2);
    p.y -= 0.2;
    float lid = max(max(-p.y, length(p.xy) - w), abs(p.z) - l);
    
    float d = min(lid, box) - texture(iChannel0, (p.xz + p.y) * 0.11).r * 0.1;
    d -= abs(abs(p.z) - l * 0.5) < 0.15 ? 0.07 : 0.0;
    
    return d;
}

float sdFloor(vec3 p) {
    float bh = textureLod(iChannel0, p.xz * rot(1.1) * 0.01, 2.5).r * 6.5;
	     bh += textureLod(iChannel0, (p.xz + vec2(12.3, 23.4)) * rot(0.5) * 0.02, 0.0).r * 1.2;
    bh /= 2.5;
    return p.y + 6.0 - bh;
}

float sdBubble(vec3 p, float t) {
    float progress = pow(min(fract(t * 0.1) * 4.5, 1.0), 2.0);
    float maxDepth = 4.2;
    float depth = maxDepth * (1.0 - progress * progress);
    float r = mix(0.01, 0.08, progress);
    
    float d = 1.2 - smoothstep(0.0, 1.0, min(progress * 5.0, 1.0)) * 0.3;
    
    return length(p + vec3(d, depth, -1.0 + 0.2 * progress * sin(progress * 10.0))) - r;
}

float sdPlant(vec3 p, float h) {
    float r = 0.02 * -(p.y + 2.5) - 0.005 * pow(sin(p.y * 30.0), 2.0);
    p.z += sin(time + h) * pow(0.2 * (p.y + 5.6), 3.0);
    return sdRod(p + vec3(0.0, 5.7, 0.0), 3.0 * h, r);
}

float sdPlants(vec3 p) {
    vec3 dd = vec3(0.2, 0.0, -0.5);
    
    // Make multiple copies, each one displaced and rotated.
    float d = 1e10;
    for (int i = 0; i < 4; i++) {
        d = min(d, min(sdPlant(p, 1.2), min(sdPlant(p + dd.xyx, 0.5), sdPlant(p + dd, 0.8))));
        p.x -= 1.0;
        p.z -= 1.0;
        p.xz *= rot(0.6);
    }
    
    return d;
}

float sdManta(vec3 p) {
    // Translate the origin to the center of the manta.
    p.xz *= rot(3.141);
    p.y += 3.5;
    p.z += 22.0;
    
    float t = mod(iTime, 20.0);
    p.x -= 30.0;
    p.xz *= rot(-t * 0.07);
    p.x += 30.0;

    if (length(p) > 3.5) return 1e7; // Ray not close enough to bother rendering.
    
    // Flap!
    p.y -= sin(-time * 1.5) * 0.2;
    p.y -= (abs(p.x) + 0.1) * sin(abs(p.x) + time * 1.5) * 0.4;
    
    // Wings.
    vec3 pp = p;
    pp.xz *= rot(3.141 / 4.0);
    float d = sdBox(pp, vec3(1.0, 0.015, 1.0));
    d = smin(d, length(p.xz * vec2(0.5, 1.0)) - 1.18, -0.05); // Nose
    
    // Eyes
    pp = p;
    if (p.y > 0.0) {
    	pp.x = abs(pp.x) - 0.1;
    	pp.z -= 0.6;
    	d = smin(d, length(pp) - 0.1, 0.05);
    }
    
    // Tail.
    p.z += 1.25;
    d = smin(d, sdBox(p, vec3(0.005, 0.005, 2.0)), 0.3);
    
    return (d - 0.02) * 0.7;
}

float godLight(vec3 p, vec3 lightPos) {
    vec3 lightDir = normalize(lightPos - p);
    vec3 sp = p + lightDir * -p.y;

    float f = 1.0 - clamp(sdSurface(sp.xz) * 10.0, 0.0, 1.0);
    f *= 1.0 - length(lightDir.xz);
    return smoothstep(0.2, 1.0, f * 0.7);
}

vec2 map(vec3 p) {
    vec3 pp = p;
    pp.xz *= rot(-.5);
    
    float surface = -p.y - sdSurface(p.xz);
    float t = time * 0.6;
    surface += (0.5 + 0.5 * (sin(p.z * 0.2 + t) + sin((p.z + p.x) * 0.1 + t * 2.0))) * 0.4;
    
    return min2(vec2(surface, 1.5),
           min2(vec2(sdChest(pp + vec3(2.0, 4.4, 0.0)), 2.5),
           min2(vec2(sdFloor(p), 3.5),
           min2(vec2(sdPlants(p - vec3(6.0, 0.0, 7.0)), 5.5),
           min2(vec2(sdManta(p), 6.5),
           min2(vec2(sdBubble(pp, time - 0.3), 4.5),
                vec2(sdBubble(pp, time), 4.5)))))));
}

vec3 calcNormal(in vec3 p) {
    vec2 e = vec2(1.0, -1.0) * 0.0025;
    return normalize(e.xyy * map(p + e.xyy).x + 
					 e.yyx * map(p + e.yyx).x + 
					 e.yxy * map(p + e.yxy).x + 
					 e.xxx * map(p + e.xxx).x);
}

float calcOcc(vec3 p, vec3 n) {
    const float dist = 0.5;
    return smoothstep(0.0, 1.0, 1.0 - (dist - map(p + n * dist).x));
}


/**********************************************************************************/


vec3 vignette(vec3 col, vec2 fragCoord) {
    vec2 q = fragCoord.xy / iResolution.xy;
    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.4);
    return col;
}

float marchGodRay(vec3 ro, vec3 rd, vec3 light, float hitDist) {
    // March through the scene, accumulating god rays.
    vec3 p = ro;
    vec3 st = rd * hitDist / 96.0;
    float god = 0.0;
    for (int i = 0; i < 96; i++) {
        float distFromGodLight = 1.0 - godLight(p, light);
        god += godLight(p, light);
        p += st;
    }
    
    god /= 96.0;

    return smoothstep(0.0, 1.0, min(god, 1.0));
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord - 0.5 * iResolution.xy) / iResolution.y;
    
    // Camera.
    vec3 ro = vec3(-0.4, -2.0, -4.0);
    ro.xz *= rot(0.03 * sin(time * 0.3));
    ro.y += sin(time * 0.2) * 0.3;
    vec3 rd = getRayDir(ro, vec3(0.0, -3.0, 0.0), uv);

    // Raymarching loop.
    int hit = 0; // ID of the object we hit.
    float d = 0.01; // Ray distance travelled.
    float maxd = 50.0; // Max ray distance.
    vec3 p;
    float outside = 1.0; // Tracks inside/outside of bubble (for refraction)
    for (float steps = 0.0; steps < 100.0; steps++) {
        p = ro + rd * d;
        vec2 h = map(p);

        if (h.x < 0.001 * d) {
            if (h.y == 4.5) {
                // Bubble refraction.
                rd = refract(rd, calcNormal(p) * sign(outside), 1.0);
                outside *= -1.0;
                continue;
            }
            
            hit = int(h.y);
            break;
        }
        
        if (d > maxd)
            break;

        d += h.x;
    }

    vec3 deepColor = vec3(0.02, 0.08, 0.2) * 0.1;
    vec3 lightPos = vec3(1.0, 4.0, 3.0);
    vec3 col = deepColor;
    if (hit > 0) {
        
        vec3 n = calcNormal(p);
        vec3 mat = vec3(0.15, 0.25, 0.6);
        if (hit == 1) {
            // Sea
            n.y = -n.y;
        } else {
        	if (hit == 2)
                mat = mix(mat, vec3(0.2, 0.15, 0.125), 0.5); // Chest
            else if (hit == 3)
                mat += vec3(0.1, 0.1, 0.0); // Sand
        	else if (hit == 5)
                mat += vec3(0.0, 0.2, 0.0); // Plant
        	else if (hit == 6)
                mat += vec3(0.5); // Manta

            mat *= 0.4 + 0.6 * godLight(p, lightPos);
            mat *= calcOcc(p, n); // Ambient occlusion.
                
            // Shadows.
            vec3 lightDir = normalize(lightPos - p);
			float sha1 = max(0.0, map(p + lightDir * 0.25).x / 0.25);
            float sha2 = max(0.0, map(p + lightDir).x);
            mat *= clamp((sha1 + sha2) * 0.5, 0.0, 1.0);
        }
        
        vec3 lightCol = vec3(1.0, 0.9, 0.8);
        vec3 lightToPoint = normalize(lightPos - p);
        
        float amb = 0.1;
        float diff = max(0.0, dot(lightToPoint, n));

        col = (amb + diff) * mat;
    }
    
    // Fog.
    float fog = clamp(pow(d / maxd * 2.0, 1.5), 0.0, 1.0);
    col = mix(col, deepColor, fog);
    
    // God rays.
    col = mix(col, vec3(0.15, 0.25, 0.3) * 12.0, marchGodRay(ro, rd, lightPos, d));
    
    // Output to screen
    col = pow(col, vec3(0.4545)); // Gamma correction
    col = vignette(col, fragCoord); // Fade screen corners
    fragColor = vec4(col, 1.0);
}