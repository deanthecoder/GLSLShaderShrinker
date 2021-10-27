// Processed by 'GLSL Shader Shrinker' (Shrunk by 1,667 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define Z0	min(iTime, 0.)
#define sat(x)	clamp(x, 0., 1.)

float g = 0.;
vec4 m;

#define AA

struct Hit {
	float d;
	int id;
	vec3 uv;
};

#define HASH	p = fract(p * .1031); p *= p + 3.3456; return fract(p * (p + p));
#define minH(a)	if (a.d < h.d) h = a

mat2 rot(float a) {
	float c = cos(a),
	      s = sin(a);
	return mat2(c, s, -s, c);
}

float sdBox(vec3 p) {
	vec3 q = abs(p) - vec3(1);
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

vec3 rayDir(vec3 ro, vec2 uv) {
	vec3 f = normalize(vec3(0, 2, 0) - ro),
	     r = normalize(cross(vec3(0, 1, 0), f));
	return normalize(f + r * uv.x + cross(f, r) * uv.y);
}

Hit map(vec3 p) {
	Hit h = Hit(length(p - vec3(0, 2.5, 0)) - 1., 1, p);
	minH(Hit(sdBox(p - vec3(0, 1, 0)), 2, p));
	minH(Hit(abs(p.y), 3, p));
	return h;
}

vec3 N(vec3 p, float t) {
	float h = t * .4;
	vec3 n = vec3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		vec3 e = .005773 * (2. * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * map(p + e * h).d;
	}

	return normalize(n);
}

float shadow(vec3 p, vec3 ld) {
	float i, h,
	      s = 1.,
	      t = .1;
	for (i = Z0; i < 30.; i++) {
		h = map(t * ld + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 20.) break;
	}

	return sat(s);
}

float ao(vec3 p, vec3 n, float h) { return map(h * n + p).d / h; }

float sss(vec3 p, vec3 ld, float h) { return smoothstep(0., 1., map(h * ld + p).d / h); }

vec3 vig(vec3 c, vec2 fc) {
	vec2 q = fc.xy / iResolution.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

float fog(vec3 v) { return exp(dot(v, v) * -.002); }

vec3 lights(vec3 p, vec3 rd, float d, Hit h) {
	vec3 c,
	     ld = normalize(vec3(6, 3, -10) - p),
	     n = N(p, d);
	float _ao, l1, l2, fre, lig,
	      ss = 0.,
	      gg = g;
	if (h.id == 1) {
		c = vec3(.5);
		ss = (sss(p, ld, .4) + sss(p, ld, 2.)) * .15;
	}
	else if (h.id == 2) c = vec3(.5, .4, .3);
	else c = vec3(.2);

	_ao = mix(ao(p, n, .2), ao(p, n, 2.), .7);
	l1 = sat(.1 + .9 * dot(ld, n)) * (.3 + .7 * shadow(p, ld)) * (.3 + .7 * _ao);
	l2 = sat(.1 + .9 * dot(ld * vec3(-1, 0, -1), n)) * .3 + pow(sat(dot(rd, reflect(ld, n))), 10.);
	fre = smoothstep(.7, 1., 1. + dot(rd, n)) * .5;
	lig = l1 + l2 * _ao + ss;
	g = gg;
	return mix(lig * c * vec3(2, 1.6, 1.4), vec3(.01), fre);
}

vec4 march(inout vec3 p, vec3 rd, float s, float mx) {
	float i,
	      d = .01;
	g = 0.;
	Hit h;
	for (i = Z0; i < s; i++) {
		h = map(p);
		if (abs(h.d) < .0015) break;
		d += h.d;
		if (d > mx) return vec4(0);
		p += h.d * rd;
	}

	return vec4(g + lights(p, rd, d, h), h.id);
}

vec3 scene(vec3 ro, vec3 rd) {
	vec3 p = ro;
	vec4 col = march(p, rd, 120., 64.);
	col.rgb *= fog(p - ro);
	if (col.w > 0.) {
		rd = reflect(rd, N(p, length(p - ro)));
		p += rd * .01;
		col += .2 * march(p, rd, 64., 10.) * fog(ro - p);
	}

	return col.rgb;
}

void mainImage(out vec4 fragColor, vec2 fc) {
	m = abs(iMouse / vec2(640, 360).xyxy);
	vec3 col,
	     ro = vec3(0, 2, -5);
	ro.yz *= rot(m.y - .5);
	ro.xz *= rot((m.x - .5) * 3.141);
	vec2 uv = (fc - .5 * iResolution.xy) / iResolution.y;
	col = scene(ro, rayDir(ro, uv));
#ifdef AA
	if (fwidth(col.r) > .01) {
		for (float dx = Z0; dx <= 1.; dx++) {
			for (float dy = Z0; dy <= 1.; dy++)
				col += scene(ro, rayDir(ro, uv + (vec2(dx, dy) - .5) / iResolution.xy));
		}

		col /= 5.;
	}

#endif
	fragColor = vec4(vig(pow(max(vec3(0), col), vec3(.45)) * sat(iTime), fc), 0);
}