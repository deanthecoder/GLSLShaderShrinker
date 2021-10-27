// Processed by 'GLSL Shader Shrinker' (Shrunk by 90 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

float n31(vec3 p) {
	const vec3 s = vec3(7, 157, 113);
	vec3 ip = floor(p);
	p = fract(p);
	p = p * p * (3. - 2. * p);
	vec4 h = vec4(0, s.yz, s.y + s.z) + dot(ip, s);
	h = mix(fract(sin(h) * 43.5453), fract(sin(h + s.x) * 43.5453), p.x);
	h.xy = mix(h.xz, h.yw, p.y);
	return mix(h.x, h.y, p.z);
}

float smin(float a, float b) {
	float h = clamp(.5 + .5 * (b - a) / -.8, 0., 1.);
	return mix(b, a, h) + .8 * h * (1. - h);
}

mat2 rot(float a) {
	float c = cos(a),
	      s = sin(a);
	return mat2(c, s, -s, c);
}

float fbm(vec3 p) { return (n31(p) + n31(p * 2.12) * .5 + n31(p * 4.42) * .25 + n31(p * 8.54) * .125 + n31(p * 16.32) * .062 + n31(p * 32.98) * .031 + n31(p * 63.52) * .0156) * .5; }

float sdBox(vec3 p, vec3 b) {
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float sdUnevenCapsule(vec2 p) {
	p.x = abs(p.x);
	float k = dot(p, vec2(.12, .99277));
	if (k < 0.) return length(p) - 1.2;
	if (k > 2.48192) return length(p - vec2(0, 2.5)) - 1.5;
	return dot(p, vec2(.99277, -.12)) - 1.2;
}

vec3 getRayDir(vec3 ro, vec2 uv) {
	vec3 f = normalize(vec3(0, 0, .8) - ro),
	     r = normalize(cross(vec3(0, 1, 0), f));
	return normalize(f + r * uv.x + cross(f, r) * uv.y);
}

float sdBoot(vec3 p) {
	p.xy = -p.xy;
	p.x /= .95 - cos((p.z + 1.2 - sign(p.x)) * .8) * .1;
	vec3 tp = p;
	tp.z = mod(tp.z - .5, .4) - .2;
	float t = max(sdBox(tp, vec3(2, .16, .12 + tp.y * .25)), sdBox(p - vec3(0, 0, 1.1), vec3(2, .16, 1.7)));
	tp = p;
	tp.x = abs(p.x) - 1.65;
	tp.z -= 1.1;
	t = min(t, sdBox(tp, vec3(.53 - .12 * tp.z, .16, 1.6)));
	p.z /= cos(p.z * .1);
	return max(max(sdUnevenCapsule(p.xz), p.y), -t);
}

float map(vec3 p) {
	float bmp = fbm(p) * (.5 + 2. * exp(-pow(length(p.xz - vec2(.5, 2.2)), 2.) * .26));
	return smin(p.y - .27 - bmp, -sdBoot(p) + (bmp * bmp * .5 - .5) * .12);
}

vec3 calcN(vec3 p, float t) {
	float h = .005 * t;
	vec3 n = vec3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		vec3 e = .5773 * (2. * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * map(p + e * h);
	}

	return normalize(n);
}

float calcShadow(vec3 p, vec3 ld) {
	float s = 1.,
	      t = .1;
	for (float i = 0.; i < 30.; i++) {
		float h = map(p + ld * t);
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001) break;
	}

	return clamp(s, 0., 1.);
}

float ao(vec3 p, vec3 n, float h) { return map(p + h * n) / h; }

vec3 vignette(vec3 c, vec2 fc) {
	vec2 q = fc.xy / iResolution.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

vec3 lights(vec3 p, vec3 rd, float d) {
	vec3 ld = normalize(vec3(6, 3, -10) - p),
	     n = calcN(p, d) + n31(p * 79.0625) * .25 - .25;
	float ao = .1 + .9 * dot(vec3(ao(p, n, .1), ao(p, n, .4), ao(p, n, 2.)), vec3(.2, .3, .5)),
	      l1 = max(0., .1 + .9 * dot(ld, n)),
	      l2 = max(0., .1 + .9 * dot(ld * vec3(-1, 0, -1), n)) * .2,
	      spe = max(0., dot(rd, reflect(ld, n))) * .1,
	      fre = smoothstep(.7, 1., 1. + dot(rd, n));
	l1 *= .1 + .9 * calcShadow(p, ld);
	return mix(.3, .4, fre) * ((l1 + l2) * ao + spe) * vec3(2, 1.8, 1.7);
}

float d = 0.;
vec3 march(vec3 ro, vec3 rd) {
	vec3 p;
	d = .01;
	for (float i = 0.; i < 96.; i++) {
		p = ro + rd * d;
		float h = map(p);
		if (abs(h) < .0015) break;
		d += h;
	}

	return lights(p, rd, d) * exp(-d * .14);
}

void mainImage(out vec4 fragColor, vec2 fc) {
	float t = mod(iTime * .2, 30.);
	vec3 ro = vec3(0, .2, -4);
	ro.yz *= rot(-sin(t * .3) * .1 - .6);
	ro.xz *= rot(1.1 + cos(t) * .2);
	fragColor = vec4(vignette(pow(march(ro, getRayDir(ro, (fc - .5 * iResolution.xy) / iResolution.y)), vec3(.45)), fc), mix(1.2, 0., (d + 1.) / 8.));
}