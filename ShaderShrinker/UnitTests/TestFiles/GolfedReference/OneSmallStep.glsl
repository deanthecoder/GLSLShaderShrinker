// Processed by 'GLSL Shader Shrinker' (Shrunk by 505 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define U	normalize
#define _f	float
#define R	iResolution

float E(v3 p) {
	const v3 s = v3(7, 157, 113);
	v3 x = floor(p);
	p = fract(p);
	p = p * p * (3. - 2. * p);
	vec4 h = vec4(0, s.yz, s.y + s.z) + dot(x, s);
	h = mix(fract(sin(h) * 43.5453), fract(sin(h + s.x) * 43.5453), p.x);
	h.xy = mix(h.xz, h.yw, p.y);
	return mix(h.x, h.y, p.z);
}

float L(_f a, _f b) {
	_f h = clamp(.5 + .5 * (b - a) / -.8, 0., 1.);
	return mix(b, a, h) + .8 * h * (1. - h);
}

mat2 H(_f a) {
	_f c = cos(a),
	   s = sin(a);
	return mat2(c, s, -s, c);
}

float m(v3 p) { return (E(p) + E(p * 2.12) * .5 + E(p * 4.42) * .25 + E(p * 8.54) * .125 + E(p * 16.32) * .062 + E(p * 32.98) * .031 + E(p * 63.52) * .0156) * .5; }

float J(v3 p, v3 b) {
	v3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float K(v2 p) {
	p.x = abs(p.x);
	_f k = dot(p, v2(.12, .99277));
	if (k < 0.) return length(p) - 1.2;
	if (k > 2.48192) return length(p - v2(0, 2.5)) - 1.5;
	return dot(p, v2(.99277, -.12)) - 1.2;
}

vec3 w(v3 G, v2 O) {
	v3 f = U(v3(0, 0, .8) - G),
	   r = U(cross(v3(0, 1, 0), f));
	return U(f + r * O.x + cross(f, r) * O.y);
}

float I(v3 p) {
	p.xy = -p.xy;
	p.x /= .95 - cos((p.z + 1.2 - sign(p.x)) * .8) * .1;
	v3 N = p;
	N.z = mod(N.z - .5, .4) - .2;
	_f t = max(J(N, v3(2, .16, .12 + N.y * .25)), J(p - v3(0, 0, 1.1), v3(2, .16, 1.7)));
	N = p;
	N.x = abs(p.x) - 1.65;
	N.z -= 1.1;
	t = min(t, J(N, v3(.53 - .12 * N.z, .16, 1.6)));
	p.z /= cos(p.z * .1);
	return max(max(K(p.xz), p.y), -t);
}

float C(v3 p) {
	_f i = m(p) * (.5 + 2. * exp(-pow(length(p.xz - v2(.5, 2.2)), 2.) * .26));
	return L(p.y - .27 - i, -I(p) + (i * i * .5 - .5) * .12);
}

vec3 j(v3 p, _f t) {
	_f h = .005 * t;
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .5773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * C(p + e * h);
	}

	return U(n);
}

float l(v3 p, v3 A) {
	_f s = 1.,
	   t = .1;
	for (_f i = 0.; i < 30.; i++) {
		_f h = C(p + A * t);
		s = min(s, 15. * h / t);
		t += h;
		if (s < .001) break;
	}

	return clamp(s, 0., 1.);
}

float g(v3 p, v3 n, _f h) { return C(p + h * n) / h; }

vec3 P(v3 c, v2 o) {
	v2 q = o.xy / R.xy;
	c *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return c;
}

vec3 B(v3 p, v3 F, _f d) {
	v3 A = U(v3(6, 3, -10) - p),
	   n = j(p, d) + E(p * 79.0625) * .25 - .25;
	_f g = .1 + .9 * dot(v3(g(p, n, .1), g(p, n, .4), g(p, n, 2.)), v3(.2, .3, .5)),
	   y = max(0., .1 + .9 * dot(A, n)),
	   z = max(0., .1 + .9 * dot(A * v3(-1, 0, -1), n)) * .2,
	   M = max(0., dot(F, reflect(A, n))) * .1,
	   v = smoothstep(.7, 1., 1. + dot(F, n));
	y *= .1 + .9 * l(p, A);
	return mix(.3, .4, v) * ((y + z) * g + M) * v3(2, 1.8, 1.7);
}

_f d = 0.;
vec3 D(v3 G, v3 F) {
	v3 p;
	d = .01;
	for (_f i = 0.; i < 96.; i++) {
		p = G + F * d;
		_f h = C(p);
		if (abs(h) < .0015) break;
		d += h;
	}

	return B(p, F, d) * exp(-d * .14);
}

void mainImage(out vec4 u, v2 o) {
	_f t = mod(iTime * .2, 30.);
	v3 G = v3(0, .2, -4);
	G.yz *= H(-sin(t * .3) * .1 - .6);
	G.xz *= H(1.1 + cos(t) * .2);
	u = vec4(P(pow(D(G, w(G, (o - .5 * R.xy) / R.y)), v3(.45)), o), mix(1.2, 0., (d + 1.) / 8.));
}