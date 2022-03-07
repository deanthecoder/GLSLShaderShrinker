// Processed by 'GLSL Shader Shrinker' (Shrunk by 2,150 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define _S	smoothstep
#define NM	normalize
#define LNG	length
#define _f	float
#define iT	iTime
#define iR	iResolution
#define F

float G(v2 p) { return fract(sin(dot(p, v2(123.45, 87.43))) * 5432.3); }

float Q(v2 p) {
#ifdef F
	p *= .05;
	return texture(iChannel1, p).r;
#else
	v2 i = floor(p);
	v2 f = fract(p);
	_f a = G(i);
	_f b = G(i + v2(1, 0));
	return mix(a, b, f.x) + (G(i + v2(0, 1)) - a) * f.y * (1. - f.x) + (G(i + v2(1)) - b) * f.x * f.y;
#endif
}

float y(v2 p) {
	_f f = .5 * Q(p * 1.1);
	f += .22 * Q(p * 2.3);
	f += .0625 * Q(p * 8.4);
	return f / .7825;
}

float sn(_f a, _f b, _f k) {
	_f h = clamp(.5 + .5 * (b - a) / k, 0., 1.);
	return mix(b, a, h) - k * h * (1. - h);
}

mat2 X(_f a) {
	_f c = cos(a),
	   s = sin(a);
	return mat2(c, s, -s, c);
}

vec2 O(v2 a, v2 b) { return a.x < b.x ? a : b; }

float SS(v3 p, v2 t) { return LNG(v2(LNG(p.xy) - t.x, p.z)) - t.y; }

float ss(v3 p) {
	p.z = mod(p.z, .6) - .3;
	return SS(p, v2(2, .2));
}

float se(v3 p) {
	_f L = LNG(p.xy);
	return max(L - 2., 1.9 - L) - y((p.xy + p.yz) * 4.) * .05;
}

float sf(v3 p) {
	p.y -= 2.5;
	return LNG(p.xy) - 1.2 + .2 * pow(abs(.5 + .5 * sin(p.z * 1.4)), 4.);
}

vec3 g(v3 p) {
	p.xz *= X(-.01 * p.z);
	if (iT < 44.) p.z += iT;
	else p.z += 44. + 2.6 * _S(0., 1., min(1., (iT - 44.) / 2.6));

	return p;
}

float Z(v3 p) {
	p = g(p);
	p.y += sin(p.x * 1.1 + sin(p.z * .7)) * .15;
	return min(p.y + 1.5 + sin(p.z) * .05, LNG(p - v3(0, -3.3, 49)) - 2.) - y(p.xz) * .1;
}

float Y(v3 p) {
	p = g(p);
	v3 T = p;
	T.x = abs(T.x) - .8;
	return sn(sn(ss(T), se(T), .1), sf(p), .3);
}

float sg(v3 p) {
	p = g(p);
	v3 T = p;
	T.z -= 49.;
	T.y++;
	T.y *= .7;
	_f d = LNG(T) - .4,
	   S = min(1., max(0., iT - 45.) * .1);
	if (iT >= 55.) S += sin(iT - 55.) * .05;
	d = sn(sn(d, -(LNG(T.xz) - p.y * .5 - mix(.1, .7, S)), -.1), SS((T - v3(0, mix(.4, .25, S), 0)).xzy, v2(.35, .04) * S), .05 * S);
	d -= y(T.xz + T.xy) * .05;
	return d;
}

float SE(v3 p) {
	_f t = min(1., max(0., iT - 62.));
	if (t <= 0.) return 1e10;
	p = g(p);
	p.z -= 49.;
	p.y += 1.5 - 1.4 * sin(t * 1.57079);
	return LNG(p) - mix(.1, 2., clamp(0., 1., max(0., iT - 62.8) * 2.5));
}

vec3 D(v3 W, v3 K, v2 UV) {
	v3 A = NM(K - W),
	   V = NM(cross(v3(0, 1, 0), A));
	return NM(A + V * UV.x + cross(A, V) * UV.y);
}

vec2 M(v3 p) { return O(O(O(v2(Y(p), 1.5), v2(Z(p), 2.5)), v2(sg(p), 3.5)), v2(SE(p), 4.5)); }

vec3 m(v3 p) {
	const v2 e = v2(1, -1) * 29e-5;
	return NM(e.xyy * M(p + e.xyy).x + e.yyx * M(p + e.yyx).x + e.yxy * M(p + e.yxy).x + e.xxx * M(p + e.xxx).x);
}

float r(v3 p) {
	v3 U = NM(v3(0, -.75, 0) - p);
	_f h,
	   P = 1.,
	   d = .01;
	for (int i = 0; i < 16; i++) {
		h = M(p + U * d).x;
		P = abs(h / d);
		if (P < .01) return 0.;
		d += h;
	}

	return P * 5.;
}

float o(v3 p, v3 n) { return 1. - (.3 - M(p + n * .3).x) * 4.; }

float u(v3 p, v3 I) {
	_f w = .1,
	   x = .02,
	   l = dot(NM(v3(0, -.75, 0) - p), -I);
	x++;
	_f st = _S(1. - w, (1. - w) * x, l) * .3;
	w *= .7;
	st = max(st, _S(1. - w, (1. - w) * 1.06, l)) * .5;
	w *= .7;
	return max(st, _S(1. - w, (1. - w) * 1.07, l));
}

vec3 ve(v3 v, v2 C) {
	v2 q = C.xy / iR.xy;
	v *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return v;
}

vec2 E() {
	_f tg = iT;
	if (tg < 2.) return v2(0, 1);
	tg -= 2.;
	if (tg < 4.) return v2(0, mix(1., 0., _S(0., 1., min(1., tg / 2.))));
	tg -= 4.;
	if (tg < 3.5) return v2(0, mix(0., -.4, _S(0., 1., min(1., tg / 1.5))));
	tg -= 3.5;
	if (tg < 4.) {
		_f f = _S(0., 1., min(1., tg / 4.));
		return v2(sin(f * 3.141) * -.6, -.4 + 1.1 * sin(f * 1.5705));
	}

	tg -= 4.;
	if (tg < 12.) return v2(0, mix(.7, -.2, _S(0., 1., min(1., tg))));
	tg -= 12.;
	if (tg < 17.) return v2(0, mix(-.2, -.05, _S(0., 1., min(1., tg / 5.))));
	tg -= 17.;
	return v2(0, mix(-.05, -.35, _S(0., 1., min(1., tg / 5.))));
}

void mainImage(out vec4 B, v2 C) {
	v2 UV = (C - .5 * iR.xy) / iR.y;
#ifdef USE_WEBCAM
	if (iT > 63.5) {
		B = vec4(mix(v3(0), texture(iChannel0, C / iR.xy).rgb, min(1., (iT - 63.5) * 5.)), 1);
		return;
	}

#endif
	v3 W, U, p, v,
	   tr = NM(v3(E(), .8));
	v2 wp = v2(sin(iT * 2.5) * .05, pow(.5 + .5 * sin(iT * 5.), 2.) * .03);
	wp *= mix(1., 0., min(1., max(0., iT - 44.) / 2.6));
	W = v3(wp, 0);
	U = D(W, tr + v3(0, .1, 0), UV);
	int H = 0;
	_f d = .01;
	for (_f steps = 0.; steps < 120.; steps++) {
		p = W + U * d;
		v2 h = M(p);
		if (h.x < .005 * d) {
			H = int(h.y);
			break;
		}

		d += h.x;
	}

	if (H > 0) {
		v3 N,
		   n = m(p),
		   J = NM(v3(0, -.75, 0) - p);
		_f sa = r(p),
		   R = o(p, n),
		   a0 = pow(max(0., dot(U, reflect(J, n))), 3.),
		   th = u(p, tr),
		   j = clamp(dot(n, -U), .01, 1.) * .05,
		   z = 1. - exp(-d * .006);
		if (H == 1) N = v3(.05, .06, .05);
		else if (H == 2) N = v3(.033, .036, .036);
		else if (H == 3) N = mix(v3(.5, .3, .2), v3(.05, .06, .05), .7);
		else if (H == 4) N = v3(0);

		v = (th * sa + (j + a0) * R) * v3(1, .9, .8);
		v *= N;
		v += th * .02 * v3(1, .9, .8);
		v = mix(v, v3(.15, .2, .25), z);
	}

	B = vec4(pow(ve(v, C), v3(.4545)), 1);
}