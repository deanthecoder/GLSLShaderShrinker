// Processed by 'GLSL Shader Shrinker' (Shrunk by 2,059 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define NM	normalize
#define _f	float
#define iR	iResolution
#define Z	min(iTime, 0.)
#define M(x)	clamp(x, 0., 1.)

_f g = 0.;
vec4 m;

#define A

struct Hit {
	_f d;
	int id;
	v3 uv;
};

#define H	p = fract(p * .1031); p *= p + 3.3456; return fract(p * (p + p));
#define F(a)	if (a.d < h.d) h = a

mat2 L(_f a) {
	_f c = cos(a),
	   s = sin(a);
	return mat2(c, s, -s, c);
}

float P(v3 p) {
	v3 q = abs(p) - v3(1);
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

vec3 I(v3 K, vec2 T) {
	v3 f = NM(v3(0, 2, 0) - K),
	   r = NM(cross(v3(0, 1, 0), f));
	return NM(f + r * T.x + cross(f, r) * T.y);
}

Hit D(v3 p) {
	Hit h = Hit(length(p - v3(0, 2.5, 0)) - 1., 1, p);
	F(Hit(P(p - v3(0, 1, 0)), 2, p));
	F(Hit(abs(p.y), 3, p));
	return h;
}

vec3 N(v3 p, _f t) {
	_f h = t * .4;
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .005773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * D(p + e * h).d;
	}

	return NM(n);
}

float Q(v3 p, v3 z) {
	_f i, h,
	   s = 1.,
	   t = .1;
	for (i = Z; i < 30.; i++) {
		h = D(t * z + p).d;
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001 || t > 20.) break;
	}

	return M(s);
}

float b(v3 p, v3 n, _f h) { return D(h * n + p).d / h; }

float S(v3 p, v3 z, _f h) { return smoothstep(0., 1., D(h * z + p).d / h); }

vec3 U(v3 c, vec2 l) {
	vec2 q = l.xy / iR.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

float k(v3 v) { return exp(dot(v, v) * -.002); }

vec3 C(v3 p, v3 J, _f d, Hit h) {
	v3 c,
	   z = NM(v3(6, 3, -10) - p),
	   n = N(p, d);
	_f _, x, y, u, B,
	   R = 0.,
	   w = g;
	if (h.id == 1) {
		c = v3(.5);
		R = (S(p, z, .4) + S(p, z, 2.)) * .15;
	}
	else if (h.id == 2) c = v3(.5, .4, .3);
	else c = v3(.2);

	_ = mix(b(p, n, .2), b(p, n, 2.), .7);
	x = M(.1 + .9 * dot(z, n)) * (.3 + .7 * Q(p, z)) * (.3 + .7 * _);
	y = M(.1 + .9 * dot(z * v3(-1, 0, -1), n)) * .3 + pow(M(dot(J, reflect(z, n))), 10.);
	u = smoothstep(.7, 1., 1. + dot(J, n)) * .5;
	B = x + y * _ + R;
	g = w;
	return mix(B * c * v3(2, 1.6, 1.4), v3(.01), u);
}

vec4 E(inout vec3 p, v3 J, _f s, _f G) {
	_f i,
	   d = .01;
	g = 0.;
	Hit h;
	for (i = Z; i < s; i++) {
		h = D(p);
		if (abs(h.d) < .0015) break;
		d += h.d;
		if (d > G) return vec4(0);
		p += h.d * J;
	}

	return vec4(g + C(p, J, d, h), h.id);
}

vec3 O(v3 K, v3 J) {
	v3 p = K;
	vec4 j = E(p, J, 120., 64.);
	j.rgb *= k(p - K);
	if (j.w > 0.) {
		J = reflect(J, N(p, length(p - K)));
		p += J * .01;
		j += .2 * E(p, J, 64., 10.) * k(K - p);
	}

	return j.rgb;
}

void mainImage(out vec4 o, vec2 l) {
	m = abs(iMouse / vec2(640, 360).xyxy);
	v3 j,
	   K = v3(0, 2, -5);
	K.yz *= L(m.y - .5);
	K.xz *= L((m.x - .5) * 3.141);
	vec2 T = (l - .5 * iR.xy) / iR.y;
	j = O(K, I(K, T));
#ifdef A
	if (fwidth(j.r) > .01) {
		for (_f dx = Z; dx <= 1.; dx++) {
			for (_f dy = Z; dy <= 1.; dy++)
				j += O(K, I(K, T + (vec2(dx, dy) - .5) / iR.xy));
		}

		j /= 5.;
	}

#endif
	o = vec4(U(pow(max(v3(0), j), v3(.45)) * M(iTime), l), 0);
}