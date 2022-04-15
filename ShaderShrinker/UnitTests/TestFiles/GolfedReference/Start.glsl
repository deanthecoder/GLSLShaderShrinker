// Processed by 'GLSL Shader Shrinker' (Shrunk by 2,088 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define RET	return
#define NM	normalize
#define _f	float
#define iR	iResolution
#define Z	min(iTime, 0.)
#define O(x)	clamp(x, 0., 1.)

_f g = 0.;
vec4 m;

#define A

struct Hit {
	_f d;
	int id;
	v3 uv;
};

#define H	p = fract(p * .1031); p *= p + 3.3456; RET fract(p * (p + p));
#define G(a)	if (a.d < h.d) h = a

mat2 M(_f a) {
	_f c = cos(a),
	   s = sin(a);
	RET mat2(c, s, -s, c);
}

float Q(v3 p) {
	v3 q = abs(p) - v3(1);
	RET length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

vec3 J(v3 L, vec2 U) {
	v3 f = NM(v3(0, 2, 0) - L),
	   r = NM(cross(v3(0, 1, 0), f));
	RET NM(f + r * U.x + cross(f, r) * U.y);
}

Hit E(v3 p) {
	Hit h = Hit(length(p - v3(0, 2.5, 0)) - 1., 1, p);
	G(Hit(Q(p - v3(0, 1, 0)), 2, p));
	G(Hit(abs(p.y), 3, p));
	RET h;
}

vec3 N(v3 p, _f t) {
	_f h = t * .4;
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .005773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * E(p + e * h).d;
	}

	RET NM(n);
}

float R(v3 p, v3 B) {
	_f i, h,
	   s = 1.,
	   t = .1;
	for (i = Z; i < 30.; i++) {
		h = E(t * B + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 20.) break;
	}

	RET O(s);
}

float j(v3 p, v3 n, _f h) { RET E(h * n + p).d / h; }

float T(v3 p, v3 B, _f h) { RET smoothstep(0., 1., E(h * B + p).d / h); }

vec3 V(v3 c, vec2 k) {
	vec2 q = k.xy / iR.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	RET c;
}

float o(v3 v) { RET exp(dot(v, v) * -.002); }

vec3 D(v3 p, v3 K, _f d, Hit h) {
	v3 c,
	   B = NM(v3(6, 3, -10) - p),
	   n = N(p, d);
	_f b, y, z, w, C,
	   S = 0.,
	   x = g;
	if (h.id == 1) {
		c = v3(.5);
		S = (T(p, B, .4) + T(p, B, 2.)) * .15;
	}
	else if (h.id == 2) c = v3(.5, .4, .3);
	else c = v3(.2);

	b = mix(j(p, n, .2), j(p, n, 2.), .7);
	y = O(.1 + .9 * dot(B, n)) * (.3 + .7 * R(p, B)) * (.3 + .7 * b);
	z = O(.1 + .9 * dot(B * v3(-1, 0, -1), n)) * .3 + pow(O(dot(K, reflect(B, n))), 10.);
	w = smoothstep(.7, 1., 1. + dot(K, n)) * .5;
	C = y + z * b + S;
	g = x;
	RET mix(C * c * v3(2, 1.6, 1.4), v3(.01), w);
}

vec4 F(inout vec3 p, v3 K, _f s, _f I) {
	_f i,
	   d = .01;
	g = 0.;
	Hit h;
	for (i = Z; i < s; i++) {
		h = E(p);
		if (abs(h.d) < .0015) break;
		d += h.d;
		if (d > I) RET vec4(0);
		p += h.d * K;
	}

	RET vec4(g + D(p, K, d, h), h.id);
}

vec3 P(v3 L, v3 K) {
	v3 p = L;
	vec4 l = F(p, K, 120., 64.);
	l.rgb *= o(p - L);
	if (l.w > 0.) {
		K = reflect(K, N(p, length(p - L)));
		p += K * .01;
		l += .2 * F(p, K, 64., 10.) * o(L - p);
	}

	RET l.rgb;
}

void mainImage(out vec4 u, vec2 k) {
	m = abs(iMouse / vec2(640, 360).xyxy);
	v3 l,
	   L = v3(0, 2, -5);
	L.yz *= M(m.y - .5);
	L.xz *= M((m.x - .5) * 3.141);
	vec2 U = (k - .5 * iR.xy) / iR.y;
	l = P(L, J(L, U));
#ifdef A
	if (fwidth(l.r) > .01) {
		for (_f dx = Z; dx <= 1.; dx++) {
			for (_f dy = Z; dy <= 1.; dy++)
				l += P(L, J(L, U + (vec2(dx, dy) - .5) / iR.xy));
		}

		l /= 5.;
	}

#endif
	u = vec4(V(pow(max(v3(0), l), v3(.45)) * O(iTime), k), 0);
}