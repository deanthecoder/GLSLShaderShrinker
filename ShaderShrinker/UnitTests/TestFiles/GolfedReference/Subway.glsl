// Processed by 'GLSL Shader Shrinker' (Shrunk by 2,908 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define TX	texture
#define NM	normalize
#define MN	min
#define LNG	length
#define _f	float
#define iT	iTime
#define iR	iResolution
#define V

const v2 b0 = v2(.36, .66);
float SN(_f a, _f b, _f k) {
	_f h = clamp(.5 + .5 * (b - a) / k, 0., 1.);
	return mix(b, a, h) - k * h * (1. - h);
}

mat2 RT(_f a) {
	_f c = cos(a),
	   s = sin(a);
	return mat2(c, s, -s, c);
}

float sx(v3 p, v3 b) {
	v3 q = abs(p) - b;
	return LNG(max(q, 0.)) + MN(max(q.x, max(q.y, q.z)), 0.);
}

float sr(v3 p, _f h, _f r) {
	v2 d = abs(v2(LNG(p.xz), p.y)) - v2(h, r);
	return MN(max(d.x, d.y), 0.) + LNG(max(d, 0.));
}

float se(v3 p, v3 a, v3 b, _f r) {
	v3 Y = p - a,
	   f = b - a;
	return LNG(Y - f * clamp(dot(Y, f) / dot(f, f), 0., 1.)) - r;
}

float sk(v3 p, _f K, _f R1, _f R2) {
	v3 q = v3(p.x, max(abs(p.y) - K, 0.), p.z);
	return LNG(v2(LNG(q.xy) - R1, q.z)) - R2;
}

vec3 F(v3 RO, v3 O, v2 UV) {
	v3 z = NM(O - RO),
	   rt = NM(cross(v3(0, 1, 0), z));
	return NM(z + rt * UV.x + cross(z, rt) * UV.y);
}

vec2 T(v2 a, v2 b) { return a.x < b.x ? a : b; }

vec2 l(v2 p, _f c, v2 I, v2 J) { return p - c * clamp(round(p / c), -I, J); }

vec3 U(v3 p) {
	p.x = abs(p.x) - 2.8;
	return p;
}

vec3 rZ(v3 p, _f c, v2 I, v2 J) {
	p.xz = l(p.xz, c, I, J);
	return p;
}

float SB(v3 p) {
	p.z -= .4;
	_f d = sx(p, v3(.385, .05, .385));
	p.y -= .01;
	p.xz = l(p.xz + v2(.06666), .13332, v2(2), v2(3));
	return MN(d, sr(p, .0225, .05));
}

float SP(v3 p) {
	p.xz = l(p.xz, .8, v2(3, 0), v2(4, 0));
	return SB(p);
}

float sb(v3 p) { return sx(p, v3(.485, .05, .485)); }

float a0(v3 p) { return sx(p - v3(0, 0, .2), v3(.485, .05, .185)); }

_f G = 0.,
   y = 1.;
vec2 sp(v3 p) {
	_f t,
	   o = sx(p, v3(1.4, .02, .02));
	if (o > 1.) return v2(o, 3.5);
	p.y += .18;
	t = sx(p, v3(1.385, .145, .02));
	o = MN(o, sx(p - v3(0, 0, .015), v3(1.4, .16, .02)));
	p.yz += v2(.18, .32);
	return T(v2(MN(o, sx(p + v3(0, .015, 0), v3(1.4, .02, .305))), 3.5), v2(MN(t, sx(p, v3(1.385, .02, .305))), 4.5));
}

vec2 ss(v3 p) { return sp(p + max(0., floor(-p.z / b0.y)) * v3(0, b0)); }

float SR(v3 p) {
	p.x -= .35;
	return max(sk(p.yxz, .1, .25, .06), p.y);
}

vec2 SS(v3 p) {
	v3 PP,
	   X = p;
	p.yz = mod(p.yz, v2(.16, .22));
	_f t,
	   d = sx(U(p), v3(.02, .145, .205)) - .015;
	PP = U(X);
	PP.xy -= v2(-.3, 1.3);
	t = 1e10;
	if (PP.y < 3.) {
		t = SN(se(PP, v3(0, 0, 1), v3(0), .1), SR(PP - v3(0, 0, .75)), .025);
		PP.yz *= RT(-atan(b0.x / b0.y));
		t = SN(MN(SN(t, SR(PP - v3(0, 0, -.75)), .0255), se(PP, v3(0), v3(0, 0, -LNG(b0 * 11.)), .1)), SR(PP - v3(0, 0, -LNG(b0 * 11.) + .75)), .025);
	}

	if (p.x > 0.) {
		const _f W = -b0.y * 12.;
		_f S = mix(W, 2.9, .5),
		   w = 2.9 - W;
		PP = X;
		PP.z -= S;
		d = max(d, sx(PP, v3(3, 1e3, w / 2.)));
	}

	return T(v2(d, 1.5), v2(t, 2.5));
}

vec2 sn(v3 p) {
	v2 t,
	   o = SS(p);
	if (p.y > 2.) return o;
	t = ss(rZ(p, 2.8, v2(1, 0), v2(1, 0)));
	_f u,
	   d = a0(rZ(p, 1., v2(3, 0), v2(3, 0)));
	p.z -= .4;
	u = SP(p);
	u -= .006 * mix(.8, 1., TX(iChannel0, p.xz * 1.7).r);
	return T(v2(MN(d, sb(rZ(p - v3(0, 0, 1.3), 1., v2(3, 0), v2(3, 5)))), 6.5), T(o, T(t, v2(u, 5.5))));
}

vec2 P(v3 p, bool uw) {
	const v3 v = v3(1.8, 4.5, 0);
	_f d = sx(p - v3(2.7, 0, 1.8), v3(.07, 10, .07)) - .01;
	d = MN(MN(MN(d, sx(p - v3(0, 5, 1.8), v3(2.8, 1, .07)) - .01), se(p, v + v3(0, 0, 2.9), v - v3(0, 0, 1e2), .18)), se(p, v + v3(0, 0, 2), v + v3(0, 0, 1.6), .22));
	v3 PP = p;
	PP.z = abs(p.z - 1.8);
	d = MN(MN(d, sk(PP - v3(1.8, 4.7, .9), .2, .2, .015)), sk(p - v3(1.8, 4.7, -2), .2, .2, .015));
	PP -= v3(0, 5, 2.5);
	d = MN(d, max(sx(PP, v3(1, .25, .06)), -sx(PP + v3(0, .3, 0), v3(.95, .2, .1))));
	if (uw) {
		_f D,
		   x = 1. - clamp((abs(p.x) - .8) / .2, 0., 1.);
		PP.y += .2;
		D = sr(PP.yxz, .05, .92);
		G += x * .001 / (.001 + D * D * .3) * mix(.01, 1., p.z < 0. ? 1. : y);
	}

	PP = p;
	PP.xz *= mat2(.70712, -.70709, .70709, .70712);
	d = MN(d, sx(PP - v3(0, 6, -20), v3(10, 1, 20. + b0 * 12.)) - TX(iChannel0, p.xz * .8).r * .01);
	v2 t = sn(p);
	p.yz -= b0 * 12.;
	p.z -= 9.;
	p.xz *= mat2(.70711, -.70711, .70711, .70711);
	p.x -= 9.08;
	return T(v2(d, 1.5), T(t, sn(p)));
}

vec3 i(v3 p) {
	const v2 e = v2(1, -1) * .5773e-4;
	return NM(e.xyy * P(p + e.xyy, false).x + e.yyx * P(p + e.yyx, false).x + e.yxy * P(p + e.yxy, false).x + e.xxx * P(p + e.xxx, false).x);
}

float g(v3 p, v3 n, _f d) { return clamp(P(p + n * d, false).x / d, 0., 1.); }

float j(v3 p, v3 N) {
	_f sd,
	   d = distance(p, N),
	   sw = 1.;
	v3 ST = (N - p) / 37.5;
	sd = LNG(ST);
	p += NM(N - p) * .01;
	for (_f i = MN(iT, 0.); i < 30.; i++) {
		p += ST;
		sw = MN(sw, max(P(p, false).x, 0.) / (sd * i));
	}

	return sw / pow(d / 20. + 1., 2.);
}

vec3 ve(v3 m, v2 B) {
	v2 q = B.xy / iR.xy;
	m *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return m;
}

vec3 E(v3 p, v3 RD, v3 n, _f H) {
	v3 R, m,
	   L = NM(v3(0, 4.5, 4.3) - p),
	   M = NM(v3(0, 4.5, -.6) - p);
	_f sc = pow(max(max(dot(reflect(L, n), RD) * y, dot(reflect(M, n), RD)), 0.), 50.);
	if (H == 3.5) R = v3(.1);
	else if (H == 4.5) R = v3(smoothstep(0., .6, TX(iChannel0, (abs(n.y) < .1 ? p.xy : p.xz) * 1.4125).r));
	else if (H == 5.5) {
		R = v3(.9, .75, .21);
		sc *= .8;
	}
	else if (H == 6.5) R = v3(mix(.3, .5, TX(iChannel0, p.xz * 1.743).r));
	else R = v3(1);

	m = R * v3(1, 1, 1.1) * ((max(max(0., dot(L, n) * y), dot(M, n)) + sc) * (j(p, v3(0, 4.5, 4.3)) * y + j(p, v3(0, 4.5, -.6))) / 2. + MN(1., .2 + g(p, n, .15) * g(p, n, .05)) * .025);
	return m + MN(G, 1.);
}

void Q(v3 RO, v3 RD, out vec3 p, out vec2 h) {
	_f d = .01;
	for (_f steps = MN(iT, 0.); steps < 60.; steps++) {
		p = RO + RD * d;
		h = P(p, true);
		if (abs(h.x) < .0015 * d) break;
		d += h.x * .9;
	}
}

void mainImage(out vec4 A, v2 B) {
	v2 h,
	   UV = (B - .5 * iR.xy) / iR.y;
	y = step(.25, sin(iT) * TX(iChannel0, v2(iT * .1)).r);
	_f C = fract(iT / 5.),
	   Z = mod(floor(iT / 5.), 3.);
	v3 RO, RD, p, n, m,
	   O = v3(0, 1, 0);
	if (Z == 0.) RO = v3(mix(0., -.5, C) * -3. - 1., -1. - 6. * mix(.5, .4, C), -10);
	else if (Z == 1.) {
		RO = v3(-3. * mix(0., .5, C) - 1., 3, -4);
		O = v3(0, 4.5, -.6);
	}
	else if (Z == 2.) {
		RO = v3(.5, -1. - 6. * (mix(.25, 0., C) - .5), -1);
		O = v3(0, 4.5, 4.3) - mix(v3(5, 3.5, -1), v3(0), C);
	}

	RD = F(RO, O, UV);
	Q(RO, RD, p, h);
	n = i(p);
	m = E(p, RD, n, h.y);
#ifdef V
	if (h.y == 2.5) {
		RD = reflect(RD, n);
		Q(p, RD, p, h);
		m = mix(m, E(p, RD, n, h.y), .75);
	}

#endif
	m *= exp(-pow(distance(RO, p) / 30., 3.) * 5.);
	A = vec4(ve(pow(m, v3(.4545)), B), 1);
}