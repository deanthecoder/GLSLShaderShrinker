// Processed by 'GLSL Shader Shrinker' (Shrunk by 866 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define S	smoothstep
#define RET	return
#define N	normalize
#define _f	float
#define R	iResolution

float u(v2 p) { RET fract(sin(dot(p, v2(123.45, 875.43))) * 5432.3); }

float A(v2 p) {
	v2 i = floor(p),
	   f = fract(p);
	_f a = u(i),
	   b = u(i + v2(1, 0)),
	   c = u(i + v2(0, 1)),
	   d = u(i + v2(1));
	f = f * f * (3. - 2. * f);
	RET mix(a, b, f.x) + (c - a) * f.y * (1. - f.x) + (d - b) * f.x * f.y;
}

float q(v2 p) {
	_f f = 0.;
	f += .5 * A(p * 1.1);
	f += .22 * A(p * 2.3);
	f += .155 * A(p * 3.9);
	f += .0625 * A(p * 8.4);
	f += .03125 * A(p * 15.);
	RET f;
}

float H(_f v) { RET S(.195, .1975, v) * S(.2, .1975, v); }

float I(v2 K) { RET H(q(K * 1e2)); }

float E(v2 L) { RET abs(2. * q(L) - 1.); }

float F(v3 p) { RET length(p - v3(0, -.8, 2)) - .7; }

float G(v3 p) {
	if (p.y > 0.) RET 1e10;
	_f h = E(p.xz * .2);
	p.xz += v2(1);
	h += .5 * E(p.xz * .8);
	h += .25 * E(p.xz * 2.);
	h += .03 * E(p.xz * 16.1);
	h *= .7 * q(p.xz);
	h -= .7;
	RET abs(p.y - h) * .6;
}

vec2 z(v3 p) {
	_f k = G(p),
	   m = F(p);
	RET k < m ? v2(k, 1) : v2(m, 2);
}

vec3 g(v3 p) {
	const v2 e = v2(1, -1) * 29e-5;
	RET N(e.xyy * z(p + e.xyy).x + e.yyx * z(p + e.yyx).x + e.yxy * z(p + e.yxy).x + e.xxx * z(p + e.xxx).x);
}

float j(v3 B, v3 x) {
	_f s = 1.,
	   d = .1;
	v3 C = N(x - B);
	while (d < 10. && s > 0.) {
		_f o = z(B + C * d).x;
		s = min(s, o / d);
		d += clamp(o, .2, 1.);
	}
	RET S(0., 1., s);
}

void mainImage(out vec4 r, v2 t) {
	v2 K = (t - .5 * R.xy) / R.y;
	v3 p, l,
	   D = N(v3(K, 1));
	_f d = .01,
	   w = 0.;
	for (_f steps = 0.; steps < 80.; steps++) {
		p = v3(0, 0, -3) + D * d;
		v2 h = z(p);
		if (abs(h.x) < .004 * d) {
			w = h.y;
			break;
		}

		if (d > 5.) break;
		d += h.x;
	}

	if (w < .5) l = v3(I(K));
	else {
		v3 J = v3(8. - 16. * iMouse.x / R.x, 6. - cos(iTime * .2), -1. - iMouse.y / 18.),
		   n = g(p),
		   y = v3(1.82, 1.8, 1.78) * dot(n, N(J - p));
		if (w > 1.5) {
			l = mix(mix(mix(v3(.05, .05, .8), v3(.05, .25, .05), S(.4, .52, q(n.xy * 3.1 + v2(iTime * .05, 0)))), v3(1), S(.8, .95, n.y) * S(.1, .8, q(n.xz * 10. + v2(iTime * .1, 0)))), v3(.3, .5, .95), S(-.5, 0., n.z));
			l *= y;
		}
		else if (w > .5) l = v3(.5) * y * pow(j(p, J), 2.);
	}

	r = vec4(pow(l, v3(.4545)), 1);
}