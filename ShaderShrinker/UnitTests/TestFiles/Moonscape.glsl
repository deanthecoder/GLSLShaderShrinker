// From https://www.shadertoy.com/view/3slfW2

float rfbm(vec2 xz) { return abs(2.0 * fbm(xz) - 1.0); }

float sdEarth(vec3 p) {
    return length(p - vec3(0.0, -0.8, 2.0)) - 0.7;
}

float sdTerrain(vec3 p) {
    if (p.y > 0.0) return 1e10; // Quick exit.

    float h = rfbm(p.xz * 0.2);
    p.xz += vec2(1.0);
    h += 0.5 * rfbm(p.xz * 0.8);
    h += 0.25 * rfbm(p.xz * 2.0);
    h += 0.03 * rfbm(p.xz * 16.1);
    
    h *= 0.7 * fbm(p.xz);
    h -= 0.7;
    
    return abs(p.y - h) * 0.6;
}

vec2 map(vec3 p) {
    float d1 = sdTerrain(p);
    float d2 = sdEarth(p);
    
    return d1 < d2 ? vec2(d1, 1.0) : vec2(d2, 2.0);
}

vec3 calcNormal(in vec3 p) {
    // Thanks iq! I didn't fancy deriving this...
    vec2 e = vec2(1.0,-1.0)*0.5773*0.0005;
    return normalize( e.xyy*map( p + e.xyy ).x + 
					  e.yyx*map( p + e.yyx ).x + 
					  e.yxy*map( p + e.yxy ).x + 
					  e.xxx*map( p + e.xxx ).x );
}

float calcShadow(vec3 origin, vec3 lightOrigin) {
    float s = 1.0;
    
    vec3 rayDir = normalize(lightOrigin - origin);
    float d = 0.1;
    while (d < 10.0 && s > 0.0) {
        float distToObj = map(origin + rayDir * d).x;
        s = min(s, distToObj / d);
        d += clamp(distToObj, 0.2, 1.0);
    }
    
    return smoothstep(0.0, 1.0, s);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord - 0.5 * iResolution.xy) / iResolution.y;

    vec3 ro = vec3(0.0, 0.0, -3.0);
    vec3 rd = normalize(vec3(uv, 1.0));
    
	float d = 0.01;
    vec3 p;
    float id = 0.0;
    for (float steps = 0.0; steps < 80.0; steps++) {
        p = ro + rd * d;
        vec2 h = map(p);
        if (abs(h.x) < 0.004 * d) {
            id = h.y;
            break;
        }
        
        if (d > 5.0) break;
         
        d += h.x;
    }
    
    vec3 col;
    if (id < 0.5) {
        // Stars.
    	col = vec3(stars(uv));
    } else {
        vec3 sunPos = vec3(8.0 - 16.0 * iMouse.x / iResolution.x, 6.0 - cos(iTime * 0.2), -1.0 - iMouse.y / 360.0 * 20.0);
        vec3 n = calcNormal(p);
        vec3 mainLight = vec3(1.82, 1.8, 1.78) * dot(n, normalize(sunPos - p));
        
        if (id > 1.5) {
            // Earth.
            vec3 sea = vec3(0.05, 0.05, 0.8);
            vec3 land = vec3(0.05, 0.25, 0.05);
            vec3 cloud = vec3(1.0);
            float landish = smoothstep(0.4, 0.52, fbm(n.xy * 3.1 + vec2(iTime * 0.05, 0.0)));
            col = mix(sea, land, landish);
            
            float cloudish = smoothstep(0.8, 0.95, n.y) * smoothstep(0.1, 0.8, fbm(n.xz * 10.0 + vec2(iTime * .1, 0.0)));
            col = mix(col, cloud, cloudish);
            
            vec3 glow = vec3(0.3, 0.5, 0.95);
            float glowish = smoothstep(-0.5, 0.0, n.z);
            col = mix(col, glow, glowish);
                
            col *= mainLight;
        } else if (id > 0.5) {
            // Terrain.
	        float shadow = pow(calcShadow(p, sunPos), 2.0);
            col = vec3(0.5) * mainLight * shadow;
        }
    }
   
    col = pow(col, vec3(0.4545));
    fragColor = vec4(col, 1.0);
}