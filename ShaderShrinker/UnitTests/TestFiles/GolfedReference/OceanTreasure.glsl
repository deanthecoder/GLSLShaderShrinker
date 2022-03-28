// Processed by 'GLSL Shader Shrinker' (Shrunk by 1,557 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define m2	mat2
#define v2	vec2
#define SS	smoothstep
#define RET	return
#define NM	normalize
#define LNG	length
#define _c	clamp
#define _f	float
#define iR	iResolution
#define te	(iTime + 37.)

float sn(_f a, _f b, _f k) {
	_f h = _c(.5 + .5 * (b - a) / k, 0., 1.);
	RET mix(b, a, h) - k * h * (1. - h);
}

mat2 N(_f a) {
	_f c = cos(a),
	   s = sin(a);
	RET m2(c, s, -s, c);
}

vec3 x(v3 M, v2 UV) {
	v3 u = NM(v3(0, -3, 0) - M),
	   L = NM(cross(v3(0, 1, 0), u));
	RET NM(u + L * UV.x + cross(u, L) * UV.y);
}

vec2 G(v2 a, v2 b) { RET a.x < b.x ? a : b; }

float O(v3 p, v3 b) {
	v3 q = abs(p) - b;
	RET LNG(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float V(v3 p, _f h, _f r) {
	p.y -= _c(p.y, 0., h);
	RET LNG(p) - r;
}

float W(v2 p) {
	_f X = texture(iChannel0, (p + v2(0, 1) * (te + 1.)) * .05).r;
	X -= texture(iChannel0, (p + v2(.7, .2) * te) * .05).r;
	RET _c(.05 + X * .2, 0., 1.);
}

float Q(v3 p) {
	if (LNG(p) > 4.5) RET 1e7;
	v3 I = p + v3(0, .6, 0);
	_f d,
	   i = O(I, v3(1, .6, 1.5));
	i = max(i, -O(I - v3(0, .9, 0), v3(1, .6, 1.5)));
	p.xy *= m2(.98007, .19867, -.19867, .98007);
	p.y -= .2;
	d = min(max(max(-p.y, LNG(p.xy) - 1.), abs(p.z) - 1.5), i) - texture(iChannel0, (p.xz + p.y) * .11).r * .1;
	d -= abs(abs(p.z) - .75) < .15 ? .07 : 0.;
	RET d;
}

float R(v3 p) {
	_f g = textureLod(iChannel0, p.xz * m2(.4536, .89121, -.89121, .4536) * .01, 2.5).r * 6.5;
	g += textureLod(iChannel0, (p.xz + v2(12.3, 23.4)) * m2(.87758, .47943, -.47943, .87758) * .02, 0.).r * 1.2;
	g /= 2.5;
	RET p.y + 6. - g;
}

float P(v3 p, _f t) {
	_f J = pow(min(fract(t * .1) * 4.5, 1.), 2.);
	RET LNG(p + v3(1.2 - SS(0., 1., min(J * 5., 1.)) * .3, 4.2 * (1. - J * J), -1. + .2 * J * sin(J * 10.))) - mix(.01, .08, J);
}

float T(v3 p, _f h) {
	_f r = .02 * -(p.y + 2.5) - .005 * pow(sin(p.y * 30.), 2.);
	p.z += sin(te + h) * pow(.2 * (p.y + 5.6), 3.);
	RET V(p + v3(0, 5.7, 0), 3. * h, r);
}

float U(v3 p) {
	const v3 o = v3(.2, 0, -.5);
	_f d = 1e10;
	for (int i = 0; i < 4; i++) {
		d = min(d, min(T(p, 1.2), min(T(p + o.xyx, .5), T(p + o, .8))));
		p.x--;
		p.z--;
		p.xz *= m2(.82534, .56464, -.56464, .82534);
	}

	RET d;
}

float S(v3 p) {
	p.xz *= m2(-1, 59e-5, -59e-5, -1);
	p.y += 3.5;
	p.z += 22.;
	_f d,
	   t = mod(iTime, 20.);
	p.x -= 30.;
	p.xz *= N(-t * .07);
	p.x += 30.;
	if (LNG(p) > 3.5) RET 1e7;
	p.y -= sin(-te * 1.5) * .2;
	p.y -= (abs(p.x) + .1) * sin(abs(p.x) + te * 1.5) * .4;
	v3 I = p;
	I.xz *= m2(.70721, .707, -.707, .70721);
	d = sn(O(I, v3(1, .015, 1)), LNG(p.xz * v2(.5, 1)) - 1.18, -.05);
	I = p;
	if (p.y > 0.) {
		I.x = abs(I.x) - .1;
		I.z -= .6;
		d = sn(d, LNG(I) - .1, .05);
	}

	p.z += 1.25;
	RET (sn(d, O(p, v3(.005, .005, 2)), .3) - .02) * .7;
}

float z(v3 p) {
	v3 C = NM(v3(1, 4, 3) - p),
	   SP = p + C * -p.y;
	_f f = 1. - _c(W(SP.xz) * 10., 0., 1.);
	f *= 1. - LNG(C.xz);
	RET SS(.2, 1., f * .7);
}

vec2 D(v3 p) {
	v3 I = p;
	I.xz *= m2(.87758, -.47943, .47943, .87758);
	_f se = -p.y - W(p.xz),
	   t = te * .6;
	se += (.5 + .5 * (sin(p.z * .2 + t) + sin((p.z + p.x) * .1 + t * 2.))) * .4;
	RET G(v2(se, 1.5), G(v2(Q(I + v3(2, 4.4, 0)), 2.5), G(v2(R(p), 3.5), G(v2(U(p - v3(6, 0, 7)), 5.5), G(v2(S(p), 6.5), G(v2(P(I, te - .3), 4.5), v2(P(I, te), 4.5)))))));
}

vec3 j(v3 p) {
	const v2 e = v2(1, -1) * .0025;
	RET NM(e.xyy * D(p + e.xyy).x + e.yyx * D(p + e.yyx).x + e.yxy * D(p + e.yxy).x + e.xxx * D(p + e.xxx).x);
}

float l(v3 p, v3 n) { RET SS(0., 1., 1. - (.5 - D(p + n * .5).x)); }

vec3 ve(v3 m, v2 w) {
	v2 q = w.xy / iR.xy;
	m *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	RET m;
}

float E(v3 M, v3 K, _f B) {
	v3 p = M,
	   ST = K * B / 96.;
	_f y = 0.;
	for (int i = 0; i < 96; i++) {
		y += z(p);
		p += ST;
	}

	y /= 96.;
	RET SS(0., 1., min(y, 1.));
}

void mainImage(out vec4 v, v2 w) {
	v2 UV = (w - .5 * iR.xy) / iR.y;
	v3 K, p, m,
	   M = v3(-.4, -2, -4);
	M.xz *= N(.03 * sin(te * .3));
	M.y += sin(te * .2) * .3;
	K = x(M, UV);
	int A = 0;
	_f d = .01,
	   H = 1.;
	for (_f steps = 0.; steps < 1e2; steps++) {
		p = M + K * d;
		v2 h = D(p);
		if (h.x < .001 * d) {
			if (h.y == 4.5) {
				K = refract(K, j(p) * sign(H), 1.);
				H *= -1.;
				continue;
			}

			A = int(h.y);
			break;
		}

		if (d > 50.) break;
		d += h.x;
	}

	m = v3(.002, .008, .02);
	if (A > 0) {
		v3 n = j(p),
		   F = v3(.15, .25, .6);
		if (A == 1) n.y = -n.y;
		else {
			if (A == 2) F = mix(F, v3(.2, .15, .125), .5);
			else if (A == 3) F += v3(.1, .1, 0);
			else if (A == 5) F += v3(0, .2, 0);
			else if (A == 6) F += v3(.5);
			F *= .4 + .6 * z(p);
			F *= l(p, n);
			v3 C = NM(v3(1, 4, 3) - p);
			_f Y = max(0., D(p + C * .25).x / .25),
			   Z = max(0., D(p + C).x);
			F *= _c((Y + Z) * .5, 0., 1.);
		}

		m = (.1 + max(0., dot(NM(v3(1, 4, 3) - p), n))) * F;
	}

	v = vec4(ve(pow(mix(mix(m, v3(.002, .008, .02), _c(pow(d / 25., 1.5), 0., 1.)), v3(1.8, 3, 3.6), E(M, K, d)), v3(.4545)), w), 1);
}