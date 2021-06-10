// Processed by 'GLSL Shader Shrinker' (Shrunk by 10 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define Z0	min(iTime, 0.)
#define sat(x)	clamp(x, 0., 1.)
#define S(x)	smoothstep(0., 1., x)

float T,
      g = 0.;

#define AA

struct Hit {
	float d;
	int id;
};

#define minH(a)	if (a.d < h.d) h = a

mat2 rot(float a) {
	vec2 cs = cos(vec2(a, a - 1.5705));
	return mat2(cs, -cs.y, cs.x);
}

vec2 polar(vec2 p, float n) {
	float t = 3.141 / n,
	      a = mod(atan(p.y, p.x) + t, 2. * t) - t;
	return length(p) * vec2(cos(a), sin(a));
}

float bx(vec3 p, vec3 b) {
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float cap(vec3 p, float h, float r) {
	p.x -= clamp(p.x, 0., h);
	return length(p) - r;
}

float sdOcta(vec3 p, float s) {
	p = abs(p);
	return (p.x + p.y + p.z - s) * .577;
}

float claws(vec3 p) {
	p.yz = polar(p.yz, 3.);
	p.x = abs(p.x) - .9;
	p.y--;
	p.xy *= rot(-.5);
	float d = max(cap(p, 1.2, .12 * S(p.x + .5)), abs(p.z) - .06);
	p.xy *= rot(-2.725);
	p.x += .1;
	p.y -= .05;
	return min(d, max(cap(p, .9, .1 * (1.1 - S(p.x - .2))), abs(p.z) - .04));
}

vec3 rayDir(vec3 ro, vec3 lookAt, vec2 uv) {
	vec3 f = normalize(lookAt - ro),
	     r = normalize(cross(vec3(0, 1, 0), f));
	return normalize(f + r * uv.x + cross(f, r) * uv.y);
}

Hit map(vec3 p) {
	p.y -= 2.;
	Hit h = Hit(abs(p.y + 2.), 1);
	vec3 op = p;
	float anim, x, d, s, i, temp, d2,
	      t = (5. - clamp(T - 3., 0., 5.)) * 1.25664;
	p.yz *= rot((sin(t) + sin(2. * t)) * sign(p.x) * 1.4);
	t = max(0., T - 8.);
	anim = sat((t - 1.) / 8.);
	vec2 shellOpen = S(vec2(anim * 4., anim * 4.5 - 3.)) * .7,
	     f = p.x - shellOpen * sign(p.x),
	     gaps = -abs(p.x) - .01 + shellOpen;
	p.x = p.x < 0. ? min(0., f.x) : max(0., f.x);
	x = abs(p.x) - .6;
	d = max(max(dot(vec2(1.5, .6), vec2(length(p.yz), -x)), -x), max(length(p.yz) - .5, 1.2 - abs(p.x)));
	minH(Hit(min(d, claws(p)), 3));
	s = length(p) - 1.;
	d = max(min(length(vec2(s, dot(sin(p * 26.), cos(p.zxy * 26.)) * .03)) - .02, abs(s + .08) - .05), gaps.x);
	p.x = op.x < 0. ? min(0., f.y) : max(0., f.y);
	i = mod(floor(atan(-p.y, p.z) * 3.183 + 10.) + 2., 20.);
	temp = sat(anim * 4. - 2.) * 20.;
	d2 = max(abs(length(p) - .65) - .05, gaps.y + .01 + .13 * step(i, temp) * min(temp - i, 1.));
	p.yz = polar(p.yz, 20.);
	p.y -= .64;
	d = min(d, max(d2, -bx(p, vec3(.15, .08, .025))));
	p = op;
	p.yz *= rot(t * .1);
	mat2 r = rot(2.5);
	d2 = 1e7;
	for (i = Z0; i < 4.; i++) {
		p -= .02;
		p.xy *= rot(3.7 + i);
		p.yz *= r;
		d2 = min(d2, sdOcta(p, .3) - .005);
	}

	g += .00008 / (.001 + d2 * d2);
	minH(Hit(d2, 4));
	p = op;
	p.x = abs(p.x);
	p.y += cos(p.x + t) * .05;
	d2 = length(p.yz) - .01;
	g += .0005 / (.001 + d2 * d2);
	minH(Hit(min(d, d2), 2));
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
	for (i = Z0; i < 15.; i++) {
		h = map(t * ld + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 10.) break;
	}

	return sat(s);
}

float ao(vec3 p, vec3 n, float h) { return map(h * n + p).d / h; }

vec3 vig(vec3 c, vec2 fc) {
	vec2 q = fc.xy / iResolution.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

float fog(vec3 v) { return exp(dot(v, v) * -.002); }

vec3 lights(vec3 p, vec3 rd, float d, Hit h) {
	vec3 c,
	     ld = normalize(vec3(6, 35, -10) - p),
	     n = N(p, d);
	float _ao, l1, l2, fre,
	      gg = g,
	      sp = 1.;
	if (h.id == 2) {
		c = vec3(.2);
		sp = 3.;
	}
	else if (h.id == 1) c = vec3(.03);
	else if (h.id == 3) c = vec3(.6);
	else c = vec3(.18, .02, .34);

	_ao = mix(ao(p, n, .2), ao(p, n, 2.), .7);
	l1 = sat(.1 + .9 * dot(ld, n)) * (.3 + .7 * shadow(p, ld)) * (.3 + .7 * _ao);
	l2 = sat(.1 + .9 * dot(ld * vec3(-1, 1, -1), n)) + pow(sat(dot(rd, reflect(ld, n))), 10.) * sp;
	fre = smoothstep(.7, 1., 1. + dot(rd, n)) * .5;
	g = gg;
	return mix((l1 * vec3(.43, .29, .52) + l2 * _ao * vec3(2.11, 1.69, 1.48)) * c, vec3(.05), fre);
}

vec4 march(inout vec3 p, vec3 rd, float s, float mx) {
	float i, pulse,
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

	pulse = mix(1., .3, (sin(T) * .5 + .5) * smoothstep(13., 15., T));
	return vec4(pow(g, pulse) * vec3(.73, .5, .88) + lights(p, rd, d, h), h.id);
}

vec3 scene(vec3 ro, vec3 rd) {
	vec3 p = ro;
	vec4 col = march(p, rd, 1e2, 50.);
	col.rgb *= fog(p - ro);
	if (col.w > 1.) {
		rd = reflect(rd, N(p, length(p - ro)));
		p += rd * .01;
		col += mix(.2, .3, col.w - 2.) * march(p, rd, 50., 10.) * fog(ro - p);
	}

	return max(vec3(0), col.rgb);
}

void mainImage(out vec4 fragColor, vec2 fc) {
	T = mod(iTime, 30.);
	vec3 col,
	     ro = mix(vec3(1, 2, -4), vec3(0, 3.5, -3), S(T / 4.));
	vec2 uv = (fc - .5 * iResolution.xy) / iResolution.y;
	col = scene(ro, rayDir(ro, vec3(0, 2, 0), uv));
#ifdef AA
	if (fwidth(col.r) > .03) {
		for (float dx = Z0; dx <= 1.; dx++) { for (float dy = Z0; dy <= 1.; dy++) col += scene(ro, rayDir(ro, vec3(0, 2, 0), uv + (vec2(dx, dy) - .5) / iResolution.xy)); }

		col /= 5.;
	}

#endif
	fragColor = vec4(vig(pow(col, vec3(.45)) * sat(iTime), fc), 1);
}