// Processed by 'GLSL Shader Shrinker' (Shrunk by 636 characters)
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
#define X(x)	clamp(x, 0., 1.)
#define S(x)	SS(0., 1., x)

_f T,
   g = 0.;

#define A

struct Hit {
	_f d;
	int id;
};

#define L(a)	if (a.d < h.d) h = a

mat2 W(_f a) {
	v2 u = cos(v2(a, a - 1.5705));
	RET mat2(u, -u.y, u.x);
}

vec2 P(v2 p, _f n) {
	_f t = 3.141 / n,
	   a = mod(atan(p.y, p.x) + t, 2. * t) - t;
	RET LNG(p) * v2(cos(a), sin(a));
}

float l(v3 p) {
	v3 q = abs(p) - v3(.15, .08, .025);
	RET LNG(MX(q, 0.)) + min(MX(q.x, MX(q.y, q.z)), 0.);
}

float k(v3 p, _f h, _f r) {
	p.x -= clamp(p.x, 0., h);
	RET LNG(p) - r;
}

float Z(v3 p) {
	p = abs(p);
	RET (p.x + p.y + p.z - .3) * .577;
}

float m(v3 p) {
	p.yz = P(p.yz, 3.);
	p.x = abs(p.x) - .9;
	p.y--;
	p.xy *= W(-.5);
	_f d = MX(k(p, 1.2, .12 * S(p.x + .5)), abs(p.z) - .06);
	p.xy *= W(-2.725);
	p.x += .1;
	p.y -= .05;
	RET min(d, MX(k(p, .9, .1 * (1.1 - S(p.x - .2))), abs(p.z) - .04));
}

vec3 R(v3 V, v2 UV) {
	v3 f = NM(v3(0, 2, 0) - V),
	   r = NM(cross(v3(0, 1, 0), f));
	RET NM(f + r * UV.x + cross(f, r) * UV.y);
}

Hit J(v3 p) {
	p.y -= 2.;
	Hit h = Hit(abs(p.y + 2.), 1);
	v3 O = p;
	_f b, x, d, s, i, tp, w,
	   t = (5. - clamp(T - 3., 0., 5.)) * 1.25664;
	p.yz *= W((sin(t) + sin(2. * t)) * sign(p.x) * 1.4);
	t = MX(0., T - 8.);
	b = X((t - 1.) / 8.);
	v2 sn = S(v2(b * 4., b * 4.5 - 3.)) * .7,
	   f = p.x - sn * sign(p.x),
	   D = -abs(p.x) - .01 + sn;
	p.x = p.x < 0. ? min(0., f.x) : MX(0., f.x);
	x = abs(p.x) - .6;
	d = MX(MX(dot(v2(1.5, .6), v2(LNG(p.yz), -x)), -x), MX(LNG(p.yz) - .5, 1.2 - abs(p.x)));
	L(Hit(min(d, m(p)), 3));
	s = LNG(p) - 1.;
	d = MX(min(LNG(v2(s, dot(sin(p * 26.), cos(p.zxy * 26.)) * .03)) - .02, abs(s + .08) - .05), D.x);
	p.x = O.x < 0. ? min(0., f.y) : MX(0., f.y);
	i = mod(floor(atan(-p.y, p.z) * 3.183 + 10.) + 2., 20.);
	tp = X(b * 4. - 2.) * 20.;
	w = MX(abs(LNG(p) - .65) - .05, D.y + .01 + .13 * step(i, tp) * min(tp - i, 1.));
	p.yz = P(p.yz, 20.);
	p.y -= .64;
	d = min(d, MX(w, -l(p)));
	p = O;
	p.yz *= W(t * .1);
	mat2 r = W(2.5);
	w = 1e7;
	for (i = z0; i < 4.; i++) {
		p -= .02;
		p.xy *= W(3.7 + i);
		p.yz *= r;
		w = min(w, Z(p) - .005);
	}

	g += 8e-5 / (.001 + w * w);
	L(Hit(w, 4));
	p = O;
	p.x = abs(p.x);
	p.y += cos(p.x + t) * .05;
	w = LNG(p.yz) - .01;
	g += 5e-4 / (.001 + w * w);
	L(Hit(min(d, w), 2));
	RET h;
}

vec3 N(v3 p, _f t) {
	_f h = t * .4;
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .005773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * J(p + e * h).d;
	}

	RET NM(n);
}

float sw(v3 p, v3 H) {
	_f i, h,
	   s = 1.,
	   t = .1;
	for (i = z0; i < 15.; i++) {
		h = J(t * H + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 10.) break;
	}

	RET X(s);
}

float j(v3 p, v3 n, _f h) { RET J(h * n + p).d / h; }

vec3 vg(v3 c, v2 y) {
	v2 q = y.xy / iR.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	RET c;
}

float z(v3 v) { RET exp(dot(v, v) * -.002); }

vec3 I(v3 p, v3 U, _f d, Hit h) {
	v3 c,
	   H = NM(v3(6, 35, -10) - p),
	   n = N(p, d);
	_f _, F, G, C,
	   E = g,
	   SP = 1.;
	if (h.id == 2) {
		c = v3(.2);
		SP = 3.;
	}
	else if (h.id == 1) c = v3(.03);
	else if (h.id == 3) c = v3(.6);
	else c = v3(.18, .02, .34);

	_ = mix(j(p, n, .2), j(p, n, 2.), .7);
	F = X(.1 + .9 * dot(H, n)) * (.3 + .7 * sw(p, H)) * (.3 + .7 * _);
	G = X(.1 + .9 * dot(H * v3(-1, 1, -1), n)) + pow(X(dot(U, reflect(H, n))), 10.) * SP;
	C = SS(.7, 1., 1. + dot(U, n)) * .5;
	g = E;
	RET mix((F * v3(.43, .29, .52) + G * _ * v3(2.11, 1.69, 1.48)) * c, v3(.05), C);
}

vec4 K(inout vec3 p, v3 U, _f s, _f M) {
	_f i,
	   d = .01;
	g = 0.;
	Hit h;
	for (i = z0; i < s; i++) {
		h = J(p);
		if (abs(h.d) < .0015) break;
		d += h.d;
		if (d > M) RET vec4(0);
		p += h.d * U;
	}

	_f Q = mix(1., .3, (sin(T) * .5 + .5) * SS(13., 15., T));
	RET vec4(pow(g, Q) * v3(.73, .5, .88) + I(p, U, d, h), h.id);
}

vec3 Y(v3 V, v3 U) {
	v3 p = V;
	vec4 o = K(p, U, 1e2, 50.);
	o.rgb *= z(p - V);
	if (o.w > 1.) {
		U = reflect(U, N(p, LNG(p - V)));
		p += U * .01;
		o += mix(.2, .3, o.w - 2.) * K(p, U, 50., 10.) * z(V - p);
	}

	RET MX(v3(0), o.rgb);
}

void mainImage(out vec4 B, v2 y) {
	T = mod(iTime, 30.);
	v3 o,
	   V = mix(v3(1, 2, -4), v3(0, 3.5, -3), S(T / 4.));
	v2 UV = (y - .5 * iR.xy) / iR.y;
	o = Y(V, R(V, UV));
#ifdef A
	if (fwidth(o.r) > .03) {
		for (_f dx = z0; dx <= 1.; dx++) {
			for (_f dy = z0; dy <= 1.; dy++)
				o += Y(V, R(V, UV + (v2(dx, dy) - .5) / iR.xy));
		}

		o /= 5.;
	}

#endif
	B = vec4(vg(pow(o, v3(.45)) * X(iTime), y), 1);
}