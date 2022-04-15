// Processed by 'GLSL Shader Shrinker' (Shrunk by 634 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define SS	smoothstep
#define RET	return
#define NM	normalize
#define MX	max
#define LNG	length
#define _f	float
#define iR	iResolution
#define z0	min(iTime, 0.)
#define Y(x)	clamp(x, 0., 1.)
#define S(x)	SS(0., 1., x)

_f T,
   g = 0.;

#define A

struct Hit {
	_f d;
	int id;
};

#define M(a)	if (a.d < h.d) h = a

mat2 X(_f a) {
	v2 w = cos(v2(a, a - 1.5705));
	RET mat2(w, -w.y, w.x);
}

vec2 Q(v2 p, _f n) {
	_f t = 3.141 / n,
	   a = mod(atan(p.y, p.x) + t, 2. * t) - t;
	RET LNG(p) * v2(cos(a), sin(a));
}

float k(v3 p) {
	v3 q = abs(p) - v3(.15, .08, .025);
	RET LNG(MX(q, 0.)) + min(MX(q.x, MX(q.y, q.z)), 0.);
}

float m(v3 p, _f h, _f r) {
	p.x -= clamp(p.x, 0., h);
	RET LNG(p) - r;
}

float sa(v3 p) {
	p = abs(p);
	RET (p.x + p.y + p.z - .3) * .577;
}

float o(v3 p) {
	p.yz = Q(p.yz, 3.);
	p.x = abs(p.x) - .9;
	p.y--;
	p.xy *= X(-.5);
	_f d = MX(m(p, 1.2, .12 * S(p.x + .5)), abs(p.z) - .06);
	p.xy *= X(-2.725);
	p.x += .1;
	p.y -= .05;
	RET min(d, MX(m(p, .9, .1 * (1.1 - S(p.x - .2))), abs(p.z) - .04));
}

vec3 U(v3 W, v2 UV) {
	v3 f = NM(v3(0, 2, 0) - W),
	   r = NM(cross(v3(0, 1, 0), f));
	RET NM(f + r * UV.x + cross(f, r) * UV.y);
}

Hit K(v3 p) {
	p.y -= 2.;
	Hit h = Hit(abs(p.y + 2.), 1);
	v3 P = p;
	_f j, x, d, s, i, tp, y,
	   t = (5. - clamp(T - 3., 0., 5.)) * 1.25664;
	p.yz *= X((sin(t) + sin(2. * t)) * sign(p.x) * 1.4);
	t = MX(0., T - 8.);
	j = Y((t - 1.) / 8.);
	v2 sn = S(v2(j * 4., j * 4.5 - 3.)) * .7,
	   f = p.x - sn * sign(p.x),
	   E = -abs(p.x) - .01 + sn;
	p.x = p.x < 0. ? min(0., f.x) : MX(0., f.x);
	x = abs(p.x) - .6;
	d = MX(MX(dot(v2(1.5, .6), v2(LNG(p.yz), -x)), -x), MX(LNG(p.yz) - .5, 1.2 - abs(p.x)));
	M(Hit(min(d, o(p)), 3));
	s = LNG(p) - 1.;
	d = MX(min(LNG(v2(s, dot(sin(p * 26.), cos(p.zxy * 26.)) * .03)) - .02, abs(s + .08) - .05), E.x);
	p.x = P.x < 0. ? min(0., f.y) : MX(0., f.y);
	i = mod(floor(atan(-p.y, p.z) * 3.183 + 10.) + 2., 20.);
	tp = Y(j * 4. - 2.) * 20.;
	y = MX(abs(LNG(p) - .65) - .05, E.y + .01 + .13 * step(i, tp) * min(tp - i, 1.));
	p.yz = Q(p.yz, 20.);
	p.y -= .64;
	d = min(d, MX(y, -k(p)));
	p = P;
	p.yz *= X(t * .1);
	mat2 r = X(2.5);
	y = 1e7;
	for (i = z0; i < 4.; i++) {
		p -= .02;
		p.xy *= X(3.7 + i);
		p.yz *= r;
		y = min(y, sa(p) - .005);
	}

	g += 8e-5 / (.001 + y * y);
	M(Hit(y, 4));
	p = P;
	p.x = abs(p.x);
	p.y += cos(p.x + t) * .05;
	y = LNG(p.yz) - .01;
	g += 5e-4 / (.001 + y * y);
	M(Hit(min(d, y), 2));
	RET h;
}

vec3 N(v3 p, _f t) {
	_f h = t * .4;
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .005773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * K(p + e * h).d;
	}

	RET NM(n);
}

float sw(v3 p, v3 I) {
	_f i, h,
	   s = 1.,
	   t = .1;
	for (i = z0; i < 15.; i++) {
		h = K(t * I + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 10.) break;
	}

	RET Y(s);
}

float l(v3 p, v3 n, _f h) { RET K(h * n + p).d / h; }

vec3 vg(v3 c, v2 z) {
	v2 q = z.xy / iR.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	RET c;
}

float B(v3 v) { RET exp(dot(v, v) * -.002); }

vec3 J(v3 p, v3 V, _f d, Hit h) {
	v3 c,
	   I = NM(v3(6, 35, -10) - p),
	   n = N(p, d);
	_f b, G, H, D,
	   F = g,
	   SP = 1.;
	if (h.id == 2) {
		c = v3(.2);
		SP = 3.;
	}
	else if (h.id == 1) c = v3(.03);
	else if (h.id == 3) c = v3(.6);
	else c = v3(.18, .02, .34);

	b = mix(l(p, n, .2), l(p, n, 2.), .7);
	G = Y(.1 + .9 * dot(I, n)) * (.3 + .7 * sw(p, I)) * (.3 + .7 * b);
	H = Y(.1 + .9 * dot(I * v3(-1, 1, -1), n)) + pow(Y(dot(V, reflect(I, n))), 10.) * SP;
	D = SS(.7, 1., 1. + dot(V, n)) * .5;
	g = F;
	RET mix((G * v3(.43, .29, .52) + H * b * v3(2.11, 1.69, 1.48)) * c, v3(.05), D);
}

vec4 L(inout vec3 p, v3 V, _f s, _f O) {
	_f i,
	   d = .01;
	g = 0.;
	Hit h;
	for (i = z0; i < s; i++) {
		h = K(p);
		if (abs(h.d) < .0015) break;
		d += h.d;
		if (d > O) RET vec4(0);
		p += h.d * V;
	}

	_f R = mix(1., .3, (sin(T) * .5 + .5) * SS(13., 15., T));
	RET vec4(pow(g, R) * v3(.73, .5, .88) + J(p, V, d, h), h.id);
}

vec3 Z(v3 W, v3 V) {
	v3 p = W;
	vec4 u = L(p, V, 1e2, 50.);
	u.rgb *= B(p - W);
	if (u.w > 1.) {
		V = reflect(V, N(p, LNG(p - W)));
		p += V * .01;
		u += mix(.2, .3, u.w - 2.) * L(p, V, 50., 10.) * B(W - p);
	}

	RET MX(v3(0), u.rgb);
}

void mainImage(out vec4 C, v2 z) {
	T = mod(iTime, 30.);
	v3 u,
	   W = mix(v3(1, 2, -4), v3(0, 3.5, -3), S(T / 4.));
	v2 UV = (z - .5 * iR.xy) / iR.y;
	u = Z(W, U(W, UV));
#ifdef A
	if (fwidth(u.r) > .03) {
		for (_f dx = z0; dx <= 1.; dx++) {
			for (_f dy = z0; dy <= 1.; dy++)
				u += Z(W, U(W, UV + (v2(dx, dy) - .5) / iR.xy));
		}

		u /= 5.;
	}

#endif
	C = vec4(vg(pow(u, v3(.45)) * Y(iTime), z), 1);
}