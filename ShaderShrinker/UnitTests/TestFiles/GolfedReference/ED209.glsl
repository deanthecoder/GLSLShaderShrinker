// Processed by 'GLSL Shader Shrinker' (Shrunk by 3,321 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define SS	smoothstep
#define NM	normalize
#define MN	min
#define MX	max
#define LNG	length
#define _c	clamp
#define ab	abs
#define _f	float
#define iR	iResolution

_f sh, gp, a0, T, S, Q, R, N, gw;
struct MarchData {
	_f d;
	v3 mat;
	_f specPower;
};

mat2 c0(_f a) {
	_f c = cos(a),
	   s = sin(a);
	return mat2(c, s, -s, c);
}

float rp(_f f, _f i1, _f i2, _f o1, _f o2) { return mix(o1, o2, _c((f - i1) / (i2 - i1), 0., 1.)); }

float sx(v3 p, v3 b) {
	v3 q = ab(p) - b;
	return LNG(MX(q, 0.)) + MN(MX(q.x, MX(q.y, q.z)), 0.);
}

float d0(v3 p, v3 r) {
	_f H = sx(p, r);
	p.xz *= mat2(.70721, .707, -.707, .70721);
	r.xz *= + 1.28234;
	return MX(H, sx(p, r));
}

float sm(v3 p) {
	const v2 h = v2(.1, .5);
	v3 q = ab(p);
	return MX(q.z - h.y, MX(q.x * .866025 + p.y * .5, -p.y) - h.x * .5);
}

float se(v3 p) {
	const v3 a = v3(0),
	         b = v3(0, 0, -.1);
	_f o = dot(b - a, b - a),
	   b0 = dot(p - a, p - a),
	   pa = dot(p - a, b - a) / o,
	   x = sqrt(b0 - pa * pa * o),
	   A = MX(0., x - ((pa < .5) ? .25 : .35)),
	   B = ab(pa - .5) - .5,
	   f = _c((.1 * (x - .25) + pa * o) / (.01 + o), 0., 1.),
	   C = x - .25 - f * .1,
	   D = pa - f;
	return ((C < 0. && B < 0.) ? -1. : 1.) * sqrt(MN(A * A + B * B * o, C * C + D * D * o));
}

float sr(v3 p, _f h, _f r) {
	v2 d = ab(v2(LNG(p.xy), p.z)) - v2(h, r);
	return MN(MX(d.x, d.y), 0.) + LNG(MX(d, 0.));
}

float SE(v3 p, v3 a, v3 b) {
	v3 PA = p - a,
	   m = b - a;
	return LNG(PA - m * _c(dot(PA, m) / dot(m, m), 0., 1.)) - .2;
}

float sn(v2 p, _f r) {
	const v3 k = v3(-.92387953, .38268343, .41421356);
	p = ab(p);
	p -= 2. * MN(dot(k.xy, p), 0.) * k.xy;
	p -= 2. * MN(dot(v2(-k.x, k.y), p), 0.) * v2(-k.x, k.y);
	p -= v2(_c(p.x, -k.z * r, k.z * r), r);
	return LNG(p) * sign(p.y);
}

vec3 gr(v3 RO, v3 lt, v2 UV) {
	v3 Z = NM(lt - RO),
	   RT = NM(cross(v3(0, 1, 0), Z));
	return NM(Z + RT * UV.x + cross(Z, RT) * UV.y);
}

MarchData MT(MarchData a, MarchData b) {
	if (a.d < b.d) return a;
	return b;
}

void sl(inout MarchData mt) {
	mt.mat = v3(.36, .45, .5);
	mt.specPower = 30.;
}

float LE(_f f) { return sin(T * 18.846 * f) * .2; }

float U() { return mix(5., -2., T); }

float W() { return ab(sin(R * 78.5375)); }

float he(v3 p) { return (LNG(p / v3(1, .8, 1)) - 1.) * .8; }

MarchData HR(v3 p, _f h, _f w) {
	w *= sin(p.x * 150.) * sin(p.y * 150.) * .002;
	MarchData rt;
	rt.d = MX(mix(sx(p, v3(1, h, 2)), he(p), .57), -p.y) - w;
	rt.mat = v3(.05);
	rt.specPower = 30.;
	return rt;
}

MarchData hr(v3 p) {
	v3 OP = p;
	MarchData r = HR(p * v3(.95, -1.4, .95), 1., 0.);
	r.d = MN(r.d, MX(MX(HR((p + v3(0, .01, 0)) * v3(.95), 1., 0.).d, p.y - .35), p.y * .625 - p.z - .66));
	p.xy *= c0(.075 * (gp - 1.) * sign(p.x));
	p.x = ab(p.x) - 1.33;
	p.y -= .1 - p.x * .1;
	r.d = MN(r.d, sx(p, v3(.4, .06 * (1. - p.x), .3 - p.x * .2)));
	p = OP;
	p.y = ab(ab(p.y + .147) - .0556) - .0278;
	r.d = MX(r.d, -sx(p + v3(0, 0, 1.5), v3(mix(.25, .55, -OP.y), .015, .1)));
	p = OP;
	p.y = ab(p.y + .16) - .06;
	p.z -= -1.1;
	r.d = MX(r.d, -MX(MX(sr(p.xzy, 1., .03), -sr(p.xzy, .55, 1.)), p.z + .2));
	sl(r);
	return r;
}

MarchData GD(v3 p) {
	MarchData r;
	sl(r);
	p.yz += v2(.1, .45);
	v3 PP = p;
	PP.z = ab(PP.z) - .5;
	r.d = MN(se(PP), sr(p, .35, .4));
	PP = v3(p.x, .28 - p.y, p.z);
	r.d = MN(r.d, sm(PP));
	PP = p;
	PP.x = ab(p.x);
	PP.xy *= mat2(.70721, .707, -.707, .70721);
	_f FS,
	   w = sign(sin(PP.z * 33.3)) * .003,
	   d = sx(PP, v3(.1 - w, .38 - w, .34)) - .02;
	PP = p - v3(0, 0, -.6);
	PP.x = ab(PP.x) - .1;
	d = MN(MN(MN(d, sr(PP, .06, .15)), sr(PP + v3(0, .12, -.05), .06, .05)), sx(p + v3(0, 0, .54), v3(.1, .06, .04)));
	if (d < r.d) {
		d = MX(d, -sr(PP + v3(0, 0, .1), .03, .2));
		r.d = d;
		r.mat = v3(.02);
	}

	FS = W();
	if (FS > .5) {
		d = sr(PP, .01 + PP.z * .05, fract(FS * 3322.423) * .5 + .9);
		if (d < r.d) {
			r.d = d;
			r.mat = v3(1);
			gw += .1 / (.01 + d * d * 4e2);
		}
	}

	return r;
}

MarchData l(v3 p) {
	MarchData r;
	sl(r);
	p.x = ab(p.x);
	p.yz += v2(.24, 0);
	p.xy *= c0(.15 * (gp - 1.));
	r.d = MN(SE(p, v3(0), v3(1.5, 0, 0)), SE(p, v3(1.5, 0, 0), v3(1.5, 0, -.3)));
	p -= v3(1.5, 0, -.3);
	p.z -= a0 * .15;
	return MT(r, GD(p));
}

float TE(v3 p) {
	p.yz += v2(.1, .32);
	return MX(sx(p, v3(.3 + .2 * (p.z - .18) - p.y * .228, .3 + .2 * cos((p.z - .18) * 3.69), .35)), .1 - p.y);
}

float Y(v3 p) {
	p.z += .8;
	p.yz *= mat2(.65244, .75784, -.75784, .65244);
	_f d = TE(p);
	p.xz *= mat2(8e-4, 1, -1, 8e-4);
	p.x -= .43;
	p.z = .25 - ab(p.z);
	return MN(d, TE(p));
}

MarchData wt(v3 p) {
	MarchData r;
	sl(r);
	p.y += .65;
	p.yz *= mat2(.98007, -.19867, .19867, .98007);
	_f w, d,
	   le = LE(1.);
	p.xy *= c0(le * .3);
	v3 PP = p;
	PP.y += .3;
	r.d = MX(sr(PP.zyx, .5, .5), p.y + .15);
	w = .5 - ab(sin(p.y * 40.)) * .03;
	d = sx(p, v3(w, .15, w));
	w = .3 - ab(sin(p.x * 40.)) * .03;
	PP.y += .18;
	d = MN(d, sr(PP.zyx, w, .75));
	PP.x = ab(PP.x);
	PP.yz *= c0(-.58525 + le * sign(p.x));
	PP.x -= .98;
	r.d = MN(MN(r.d, MX(sr(PP.zyx, .4, .24), -PP.y)), sx(PP, v3(.24, .2, .14 + .2 * PP.y)));
	p = PP;
	PP.xz = ab(PP.xz) - v2(.12, .25);
	r.d = MN(r.d, MX(MN(sr(PP.xzy, .1, .325), sr(PP.xzy, .05, .5)), PP.y));
	p.y += .68;
	r.d = MN(r.d, sx(p, v3(sign(ab(p.y) - .04) * .005 + .26, .2, .34)));
	if (d < r.d) {
		r.d = d;
		r.mat = v3(.02);
	}

	return r;
}

MarchData ls(v3 p) {
	MarchData r;
	sl(r);
	_f le = LE(1.);
	p.z += .27;
	p.yz *= c0(le * sign(p.x));
	p.z -= .27;
	p.y += .65;
	p.yz *= mat2(.98007, -.19867, .19867, .98007);
	p.xy *= c0(le * .3);
	v3 G,
	   PP = p;
	PP.x = ab(PP.x);
	PP.y += .48;
	PP.yz *= mat2(.83357, -.55241, .55241, .83357);
	PP.x -= .98;
	G = PP;
	p = PP;
	PP.xz = ab(PP.xz) - v2(.12, .25);
	p.y += .68;
	p.xy = ab(p.xy) - .12;
	_f SR = sx(p, v3(.07, .05, 1.2));
	G -= v3(0, -.7, 0);
	r.d = sx(G - v3(0, 0, 1.15), v3(.17, .17, .07)) - .04;
	G.z++;
	r.d = MN(MN(r.d, d0(G.xzy, v2(.28 - sign(ab(G.z) - .3) * .01, .5).xyx)), Y(G));
	if (SR < r.d) {
		r.d = SR;
		r.mat = v3(.8);
	}

	return r;
}

MarchData P(v3 p) {
	p.yz += v2(LE(2.) * .2 + .1, -U());
	MarchData r = ls(p);
	_f f = MN(sh * 2., 1.),
	   e0 = f < .5 ? SS(0., .5, f) : (1. - SS(.5, 1., f) * .2);
	p.yz -= e0 * .5;
	gp = SS(0., 1., _c((sh - .66) * 6., 0., 1.));
	a0 = SS(0., 1., _c((sh - .83) * 6., 0., 1.)) + W() * .5;
	r = MT(r, wt(p));
	p.yz *= c0(.1 * (-Q + LE(2.) + SS(0., 1., _c((sh - .5) * 6., 0., 1.)) - 1.));
	p.xz *= c0(S * .2);
	return MT(MT(MT(r, hr(p)), HR(p, .8, 1.)), l(p));
}

MarchData rm(v3 p) {
	const v3 FR = v3(2.8, 2.6, .1);
	MarchData r;
	r.mat = v3(.4);
	r.specPower = 1e7;
	v2 XY = p.xy - v2(0, 2);
	p.x = ab(p.x);
	p.yz += v2(.5, -3.4);
	_f K, d,
	   M = sx(p, FR + v3(0, 0, 1)),
	   u = LNG(p.z - 8.);
	r.d = MN(u, MX(LNG(p.z), -M + .1));
	if (r.d == u) if (MN(MX(MN(ab(sn(XY, 2.6)), ab(sn(XY, 1.9))), MN(.7 - ab(XY.x + 1.2), -XY.y)), MX(ab(sn(XY, 1.2)), MN(XY.x, .7 - ab(XY.y)))) < .3) r.mat = v3(.39, .57, .71);
	_f L = MX(sx(p, FR + v3(.4, .4, .1)), -M),
	   O = FR.x * .5;
	p.x -= FR.x;
	p.xz *= c0(N * 2.1);
	p.x += O;
	K = sx(p, v3(O, FR.yz));
	p = ab(p) - v3(O * .5, 1.1, .14);
	d = MN(L, MX(K, -MX(sx(p, v3(.45, .9, .1)), -sx(p, v3(.35, .8, 1)))));
	if (d < r.d) {
		r.d = d;
		r.mat = v3(.02, .02, .024);
		r.specPower = 10.;
	}

	return r;
}

MarchData mp(v3 p) {
	MarchData r = MT(rm(p), P(p));
	_f gd = LNG(p.y + 3.);
	if (gd < r.d) {
		r.d = gd;
		r.mat = v3(.1);
	}

	return r;
}

float z(v3 p) {
	v3 RD = NM(v3(10, 10, -10) - p);
	_f rs = 1.,
	   t = .1;
	for (_f i = 0.; i < 30.; i++) {
		_f h = mp(p + RD * t).d;
		rs = MN(rs, 12. * h / t);
		t += h;
		if (rs < .001 || t > 25.) break;
	}

	return _c(rs, 0., 1.);
}

vec3 y(v3 p, _f t) {
	_f d = .01 * t * .33;
	v2 e = v2(1, -1) * .5773 * d;
	return NM(e.xyy * mp(p + e.xyy).d + e.yyx * mp(p + e.yyx).d + e.yxy * mp(p + e.yxy).d + e.xxx * mp(p + e.xxx).d);
}

float i(v3 p, v3 n) { return mp(p + .33 * n).d / .33; }

vec3 ve(v3 E, v2 fd) {
	v2 q = fd.xy / iR.xy;
	E *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return E;
}

vec3 j(v3 p, v3 RD, _f d, MarchData I) {
	v3 h0 = NM(v3(10, 10, -10) - p),
	   n = y(p, d);
	_f py = MX(0., dot(h0, n)),
	   v = MX(0., dot(-h0, n)) * .2,
	   f0 = pow(MX(0., dot(RD, reflect(h0, n))), I.specPower) * 2.,
	   fe = SS(.7, 1., 1. + dot(RD, n)),
	   X = exp(-LNG(p) * .05);
	py *= mix(.2, 1., z(p));
	return mix(I.mat * ((py + v) * i(p, n) + f0) * v3(2, 1.6, 1.7), v3(.01), fe) * X;
}

vec3 GR(v3 RO, v3 RD) {
	v3 p;
	_f g,
	   d = .01;
	MarchData h;
	for (_f steps = 0.; steps < 120.; steps++) {
		p = RO + RD * d;
		h = mp(p);
		if (ab(h.d) < .0015 * d) break;
		if (d > 64.) return v3(0);
		d += h.d;
	}

	g = gw;
	return j(p, RD, d, h) + W() * .3 + g;
}

void mainImage(out vec4 fr, v2 fd) {
	T = 1.;
	S = 0.;
	Q = 0.;
	R = 0.;
	N = 1.;
	sh = 1.;
	v3 RO, lt, E;
	_f g0, V, J,
	   te = mod(iTime, 55.);
	if (te < 12.) {
		g0 = 0.;
		V = 12.;
		T = 0.;
		RO = v3(0, -1.5, -.625);
		lt = v3(0, -1, U());
		N = SS(0., 1., te / 5.);
		sh = rp(te, 7., 10., 0., 1.);
	}
	else if (te < 25.) {
		g0 = 12.;
		V = 25.;
		_f t = te - g0;
		T = SS(0., 1., rp(t, 3., 8., 0., 1.));
		RO = v3(-.5 * cos(t * .7), .5 - t * .1, U() - 3.);
		lt = v3(0, 0, U());
	}
	else if (te < 29.) {
		g0 = 25.;
		V = 29.;
		RO = v3(-2, .5 + (te - g0) * .1, U() - 3.);
		lt = v3(0, 0, U());
	}
	else if (te < 37.) {
		g0 = 29.;
		V = 37.;
		_f t = te - g0;
		RO = v3(1.5, -1. - t * .05, U() - 5.);
		lt = v3(0, -1, U());
		sh = rp(t, 2., 5., 1., 0.);
	}
	else if (te < 55.) {
		g0 = 37.;
		V = 55.;
		_f t = te - g0;
		RO = v3(-1.8, -.5, U() - 2.5);
		sh = rp(t, 2., 3., 0., 1.) - rp(t, 11.5, 14.5, 0., 1.);
		lt = v3(0, sh * .5 - .5, U());
		S = rp(t, 3., 3.2, 0., 1.) * sh;
		Q = rp(t, 3.2, 3.4, 0., 1.) * sh;
		R = t <= 9.5 ? rp(t, 4., 9.5, 0., 1.) : 0.;
	}

	J = 1. - cos(MN(1., 2. * MN(ab(te - g0), ab(te - V))) * 1.5705);
	E = v3(0);
#ifdef AA
	for (_f dx = 0.; dx <= 1.; dx++) {
		for (_f dy = 0.; dy <= 1.; dy++) {
			v2 F = fd + v2(dx, dy) * .5;
#else
			v2 F = fd;
#endif
			F += (fract(W() * v2(23242.232, 978.23465)) - .5) * 10.;
			v2 UV = (F - .5 * iR.xy) / iR.y;
			E += GR(RO, gr(RO, lt, UV));
#ifdef AA
		}
	}

	E /= 4.;
#endif
	fr = vec4(ve(pow(E * J, v3(.4545)), fd), 1);
}