// Processed by 'GLSL Shader Shrinker' (Shrunk by 1,388 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define SS	smoothstep
#define NM	normalize
#define _f	float
#define iR	iResolution
#define W	v3(.6, .8, .9)
#define T	- 1e2

float y(v2 p) { return fract(sin(dot(p, v2(123.45, 875.43))) * 5432.3); }

float D(v2 p) {
	v2 i = floor(p),
	   f = fract(p);
	_f a = y(i),
	   b = y(i + v2(1, 0)),
	   c = y(i + v2(0, 1)),
	   d = y(i + v2(1));
	f = f * f * (3. - 2. * f);
	return mix(a, b, f.x) + (c - a) * f.y * (1. - f.x) + (d - b) * f.x * f.y;
}

float t(v2 p) {
	_f s = 0.,
	   a = .498,
	   f = 1.012;
	for (int i = 0; i < 8; i++) {
		s += a * D(p * f);
		a *= .501;
		f *= 1.988;
	}

	return s;
}

vec2 L(v2 p) { return mod(p, v2(2)) - v2(1); }

float O(v3 p, v3 r) {
	p = abs(p) - r;
	return length(max(p, 0.)) + min(max(p.x, max(p.y, p.z)), 0.);
}

float N(v3 p, _f r) { return length(p.xz) - r; }

float R(v3 p) {
	_f d,
	   r = .6;
	r -= .1 * p.y / 4.;
	r -= .05 * pow(.5 + .5 * sin(p.y * 4.5), 2e2);
	r -= .03 * pow(.5 + .5 * sin(15. * atan(p.z, p.x)), 4.);
	d = N(p, r);
	{
		v3 q = p;
		q.y -= 4.16;
		d = min(d, O(q, v3(.7, .16, .7)) - .05);
	}
	{
		v3 q = p;
		q.y += 3.6;
		d = min(d, O(q, v3(.7, .16, .7)) - .05);
	}
	return d;
}

float Q(v3 p) {
	_f d = 1E10;
	v3 q = p;
	q.xz = L(q.xz);
	{ d = max(max(R(q), -O(p, v3(4, 50, 6))), O(p, v3(6, 4, 8))); }
	{
		q.y += 4.;
		d = max(min(d, O(q, v3(.9, .16, .9)) - .05), O(p, v3(8, 5, 8)));
	}
	{
		v3 J = p;
		J.xz -= v2(1);
		J.xz = L(J.xz);
		J.y += 5.3;
		d = max(min(d, O(J, v3(.9, 1, .9)) - .05), O(p, v3(9, 6, 9)));
		{
			J.y = p.y - 4.6;
			d = min(d, max(O(J, v3(.9, .5, .85)) - .05, O(p, v3(6.5, 6, 9)) - .05));
		}
	}
	d = min(d, O(p - v3(0, 5.3, 0), v3(7, .2, 9.5) - .05));
	d += t((p.xy + p.yz) * 1.2) * .1;
	return d * .8;
}

float S(v3 p) {
	p.y -= T;
	return p.y - mix(70. * (pow(t(p.xz * v2(.015)), 1.5) - .5), -T + (.7 * t((p.xz + v2(0, 140)) * v2(.05)) - .538) * 20., clamp(SS(10., 0., length(p.xz) / 15.), 0., 1.));
}

float P(v3 p) { return p.y - T + (.5 - t(p.xz * .04)) * 2.; }

vec2 B(v3 p) {
	_f E = Q(p - v3(0, 2.35, 0)),
	   F = S(p),
	   G = P(p);
	v2 z = v2(E, 1);
	if (F < z.r) z = v2(F, 2);
	if (G < z.r) z = v2(G, 3);
	return z;
}

vec3 j(v3 p) {
	v3 n = v3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		v3 e = .5773 * (2. * v3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * B(p + 5e-4 * e).x;
	}

	return NM(n * .5773);
}

float k(v3 I, v3 A, _f x) {
	_f s = 1.,
	   d = 1.;
	v3 K = NM(A - I);
	while (d < 20. && s > 0.) {
		_f o = B(I + K * d).x;
		s = min(s, .5 + .5 * o / (x * d));
		d += clamp(o, .2, 1.);
	}
	return SS(0., 1., s);
}

float l(v3 I, v3 n) {
	_f H = 0.,
	   d = .01;
	for (int i = 1; i < 5; i++) {
		_f h = B(I + n * d).x;
		H += d - h;
		d += .15;
	}

	return 1. - clamp(H * .5, 0., 1.);
}

vec3 ve(v3 m, v2 w) {
	v2 q = w.xy / iR.xy;
	m *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return m;
}

void mainImage(out vec4 v, v2 w) {
	v2 z = v2(0),
	   UV = (w - .5 * iR.xy) / iR.y;
	v3 p,
	   u = v3(-.59359, -.31163, .74198),
	   M = NM(cross(v3(0, 1, 0), u)),
	   K = NM(u + UV.x * M + UV.y * cross(u, M)),
	   Z = v3(.42563, .68101, .59588),
	   Y = Z * 1e2,
	   m = v3(0);
	_f d = 0.;
	for (int steps = min(iFrame, 0); steps < 200; steps++) {
		p = v3(7, 6.5, -25) + K * d;
		z = B(p);
		_f o = z.x;
		if (abs(o) < d * .001) break;
		if (d > 12e2) {
			z = v2(0);
			break;
		}

		d += o;
	}

	if (z.y < .5) m = W;
	else {
		v3 C,
		   n = j(p);
		_f U, X, g, V,
		   H = l(p, n);
		if (z.y < 1.5) {
			U = k(p, Y, .125);
			C = v3(.2, .18, .1);
		}
		else if (z.y < 2.5) {
			C = mix(v3(.04, .04, .03), v3(.025, .1, .03), SS(.7, 1., n.y));
			U = k(p, Y, 1.);
		}
		else if (z.y < 3.5) {
			C = v3(.002, .02, .04);
			U = k(p, Y, 1.);
			_f tt = S(p);
			C *= 1. * (1. + SS(30., 0., tt));
			C *= 1. * (1. + SS(5., 0., tt));
		}

		X = max(0., dot(Z, n)) * 6.;
		g = .2 * max(0., dot(Z * v3(-1, 0, -1), n));
		V = (.6 + .5 * n.y) * .05;
		m = C * X * v3(1.64, 1.27, .99) * U;
		m += C * g * v3(1.64, 1.27, .99) * H;
		m += C * V * W * H;
		m = mix(m, W, pow(length(p - v3(7, 6.5, -25)) / 12e2, 7.));
	}

	v = vec4(ve(pow(m, v3(.4545)), w), 1);
}