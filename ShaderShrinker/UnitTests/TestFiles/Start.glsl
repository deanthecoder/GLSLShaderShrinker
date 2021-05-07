// 'Title' dean_the_coder (Twitter: @deanthecoder)
// URL (YouTube: URL)
//
// Blurb
//
// Tricks to get the performance:
//
// Thanks to Evvvvil, Flopine, Nusan, BigWings, Iq, Shane,
// Blackle and a bunch of others for sharing their knowledge!

// TODO

// Find futherest point in shadow code, and stop of exceeded.
// Same for raymarch?

// Hit -> vec?
// Simplify maths.
// Set sceneAdjust
// Add quick bounding checks.
// Do we need max draw distance? (MAX_DIST)
// Reduce MAX_DIST
// Reduce MAX_STEPS
// Reduce SHADOW_STEPS
// Quick 'max dist' check to prevent rendering shadows too far?
// Remove AO?
// Shorten common long function names.

#define MIN_DIST		 .0015
#define MAX_DIST		 64.0
#define MAX_STEPS		 120.0
#define MAX_RDIST		 10.0
#define MAX_RSTEPS		 64.0
#define SHADOW_STEPS	 30.0
#define MAX_SHADOW_DIST  20.0

#define Z0 min(iTime, 0.)
#define sat(x) clamp(x, 0., 1.)

float g = 0.0; // todo - Remove?
vec4 m; // todo - Remove

#define AA    // Enable this line if your GPU can take it!

struct Hit {
	float d;
	int id;  // todo - Needed?
	vec3 uv; // todo - Needed?
};

// Thnx Dave_Hoskins - https://www.shadertoy.com/view/4djSRW
#define HASH    p = fract(p * .1031); p *= p + 3.3456; return fract(p * (p + p));
float hash11(float p) { HASH }

vec2 hash22(vec2 p) { HASH }

float hash31(vec3 p3)
{
	p3  = fract(p3 * .1031);
    p3 += dot(p3, p3.yzx + 3.3456);
    return fract((p3.x + p3.y) * p3.z);
}

float hash21(vec2 p) { return hash31(p.xyx); }

vec4 hash44(vec4 p) { HASH }

float n31(vec3 p) {
    // Thanks Shane - https://www.shadertoy.com/view/lstGRB
	const vec3 s = vec3(7, 157, 113);
	vec3 ip = floor(p);
	p = fract(p);
	p = p * p * (3. - 2. * p);

	vec4 h = vec4(0, s.yz, s.y + s.z) + dot(ip, s);
	h = mix(hash44(h), hash44(h + s.x), p.x);

	h.xy = mix(h.xz, h.yw, p.y);
	return mix(h.x, h.y, p.z);
}

float n21(vec2 p) { return n31(vec3(p, 1)); }

float fbm(vec3 p)
{
    float a = 0.0, b = 0.5, i;
    for (i = Z0; i < 4.0; i++)
    {
        a += b * n31(p);
        b *= 0.5;
        p *= 2.0;
    }
    
	return a * 0.5;
}

float smin(float a, float b, float k) {
	float h = sat(.5 + .5 * (b - a) / k);
	return mix(b, a, h) - k * h * (1. - h);
}

#define minH(a) if (a.d < h.d) h = a

float remap(float f, float in1, float in2, float out1, float out2) {
	return mix(out1, out2, sat((f - in1) / (in2 - in1)));
}

mat2 rot(float a) {
	float c = cos(a), s = sin(a);
	return mat2(c, s, -s, c);
}

vec3 rotAx(vec3 p, vec3 ax, float a) {
	// Thanks Blackle.
	return mix(dot(ax, p) * ax, p, cos(a)) + cross(ax, p) * sin(a);
}

float opRep(float p, float c) {
	float c2 = c * .5;
	return mod(p + c2, c) - c2;
}

vec2 opModPolar(vec2 p, float n, float o)
{
	float angle = 3.141 / n,
		  a = mod(atan(p.y, p.x) + angle + o, 2. * angle) - angle;
	return length(p) * vec2(cos(a), sin(a));
}

float sdHex(vec2 p, float r) {
	p = abs(p);
	return -step(max(dot(p, normalize(vec2(1, 1.73))), p.x), r);
}

float sdBox(vec3 p, vec3 b) {
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float sdCyl(vec3 p, vec2 hr) {
	vec2 d = abs(vec2(length(p.xy), p.z)) - hr;
	return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdCapsule(vec3 p, float h, float r) {
	p.x -= clamp(p.x, 0., h);
	return length(p) - r;
}

vec3 rayDir(vec3 ro, vec3 lookAt, vec2 uv) {
	vec3 f = normalize(lookAt - ro),
		 r = normalize(cross(vec3(0, 1, 0), f));
	return normalize(f + r * uv.x + cross(f, r) * uv.y);
}

#define SPHERE_ID  1
#define BOX_ID     2
#define GROUND_ID  3

// Map the scene using SDF functions.
Hit map(vec3 p) {
	Hit h = Hit(length(p - vec3(0, 2.5, 0)) - 1., SPHERE_ID, p);
    
    minH(Hit(sdBox(p - vec3(0, 1, 0), vec3(1)), BOX_ID, p));
    minH(Hit(abs(p.y), GROUND_ID, p));

	return h;
}

vec3 N(vec3 p, float t) {
	const float sceneAdjust = 0.4; // todo - inline
	float h = t * sceneAdjust;
	vec3 n = vec3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		vec3 e = .005773 * (2. * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * map(p + e * h).d;
	}

	return normalize(n);
}

float shadow(vec3 p, vec3 ld) {
	// Thanks iq.
	float s = 1., t = .1, i, h;
	for (i = Z0; i < SHADOW_STEPS; i++)
	{
		h = map(t * ld + p).d; // todo - Need the entire SDF?
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > MAX_SHADOW_DIST) break;
	}

	return sat(s);
}

// Quick ambient occlusion.
float ao(vec3 p, vec3 n, float h) { return map(h * n + p).d / h; }

// Sub-surface scattering. (Thanks Evvvvil)
float sss(vec3 p, vec3 ld, float h) { return smoothstep(0.0, 1.0, map(h * ld + p).d / h); }


/**********************************************************************************/

vec3 vig(vec3 c, vec2 fc) {
	vec2 q = fc.xy / iResolution.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

float fog(vec3 v) { return exp(dot(v, v) * -0.002); }

vec3 lights(vec3 p, vec3 rd, float d, Hit h) {
	vec3 ld = normalize(vec3(6, 3, -10) - p),
		 n = N(p, d), c;
    float ss = 0.0, gg = g;
         
    if (h.id == SPHERE_ID) { 
        c = vec3(0.5);
        ss = (sss(p, ld, 0.4) + sss(p, ld, 2.0)) * 0.15;
    } else if (h.id == BOX_ID) c = vec3(0.5, 0.4, 0.3);
    else c = vec3(0.2);
    
	float _ao = mix(ao(p, n, .2), ao(p, n, 2.), .7),

	// Primary light.
	l1 = sat(.1 + .9 * dot(ld, n))
         * (0.3 + 0.7 * shadow(p, ld)) // ...with shadow.
         * (0.3 + 0.7 * _ao), // ...and _some_ AO.

	// Secondary(/bounce) light.
	l2 = sat(.1 + .9 * dot(ld * vec3(-1, 0, -1), n)) * .3

	// Specular.
	     + pow(sat(dot(rd, reflect(ld, n))), 10.0),

	// Fresnel
	fre = smoothstep(.7, 1., 1. + dot(rd, n)) * 0.5;

	// Combine into final color.
	float lig = l1 + l2 * _ao + ss;
    g = gg;
	return mix(lig * c * vec3(2, 1.6, 1.4), // 2, 1.8, 1.7 - White light
               vec3(0.01), // Fresnel edge color. todo Needed?
               fre);
}

vec4 march(inout vec3 p, vec3 rd, float s, float mx) {
	float d = .01, i;
    g = 0.0;
	Hit h;
	for (i = Z0; i < s; i++) {
		h = map(p);

		if (abs(h.d) < MIN_DIST)
			break;

        d += h.d;
        if (d > mx)
            return vec4(0);

        p += h.d * rd; // No hit, so keep marching.
	}

	return vec4(g + lights(p, rd, d, h), h.id);
}

vec3 scene(vec3 ro, vec3 rd) {
    vec3 p = ro;
    vec4 col = march(p, rd, MAX_STEPS, MAX_DIST);
    col.rgb *= fog(p - ro);
    
    if (col.w > 0.0) { // todo - id0 doesn't reflect.
        rd = reflect(rd, N(p, length(p - ro)));
        p += rd * 0.01;
        col += 0.2
               * march(p, rd, MAX_RSTEPS, MAX_RDIST)
               * fog(ro - p);
    }
    
    return col.rgb;
}

void mainImage(out vec4 fragColor, vec2 fc)
{
	m = abs(iMouse / vec2(640, 360).xyxy);

	// Camera.
	/*
	float t = min(iTime, abs(iTime - 8.)),
		  dim = 1. - pow(abs(cos(clamp(t, -1., 1.) * 1.57)), 10.);

	vec3 cam;

	if (iTime < 8.) {
		vec3 p1 = vec3((m.x - .5) * 2., m.y, 25. * w),
			 p2 = vec3((m.x - .5) * 2., m.y, 25. * w);
		cam = mix(p1, p2, remap(iTime, 0., 8., 0., 1.));
	}

	vec3 ro = vec3(0, 0, -cam.z);
	ro.yz *= rot(cam.y * -1.4);
	ro.xz *= rot(cam.x * -3.141);
*/
	vec3 col, lookAt = vec3(0, 2, 0), ro = vec3(0, 2, -5); // todo - inline?
	ro.yz *= rot(m.y - .5);
	ro.xz *= rot((m.x - .5) * 3.141);

    vec2 uv = (fc - .5 * iResolution.xy) / iResolution.y;
    col = scene(ro, rayDir(ro, lookAt, uv));

#ifdef AA
    if (fwidth(col.r) > 0.01) { // todo - Tweak
        for (float dx = Z0; dx <= 1.; dx++)
            for (float dy = Z0; dy <= 1.; dy++)
                col += scene(ro, rayDir(ro, lookAt, uv + (vec2(dx, dy) - 0.5) / iResolution.xy));
        col /= 5.;
    }
#endif

	// Output to screen.
	fragColor = vec4(vig(pow(max(vec3(0), col), vec3(.45)) * sat(iTime), fc), 0);
}
