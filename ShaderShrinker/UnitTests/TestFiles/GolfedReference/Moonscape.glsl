// Processed by 'GLSL Shader Shrinker' (Shrunk by 676 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define v3	vec3
#define v2	vec2
#define S	smoothstep
#define N	normalize
#define F	float
#define R	iResolution

float v(v2 A) { return abs(2. * fbm(A) - 1.); }

float w(v3 p) { return length(p - v3(0, -.8, 2)) - .7; }

float x(v3 p) {
	if (p.y > 0.) return 1e10;
	F h = v(p.xz * .2);
	p.xz += v2(1);
	h += .5 * v(p.xz * .8);
	h += .25 * v(p.xz * 2.);
	h += .03 * v(p.xz * 16.1);
	h *= .7 * fbm(p.xz);
	h -= .7;
	return abs(p.y - h) * .6;
}

vec2 q(v3 p) {
	F f = x(p),
	  g = w(p);
	return f < g ? v2(f, 1) : v2(g, 2);
}

vec3 c(v3 p) {
	const v2 e = v2(1, -1) * 29e-5;
	return N(e.xyy * q(p + e.xyy).x + e.yyx * q(p + e.yyx).x + e.yxy * q(p + e.yxy).x + e.xxx * q(p + e.xxx).x);
}

float a(v3 r, v3 m) {
	F s = 1.,
	  d = .1;
	v3 t = N(m - r);
	while (d < 10. && s > 0.) {
		F i = q(r + t * d).x;
		s = min(s, i / d);
		d += clamp(i, .2, 1.);
	}
	return S(0., 1., s);
}

void mainImage(out vec4 j, v2 l) {
	v2 z = (l - .5 * R.xy) / R.y;
	v3 p, b,
	   u = N(v3(z, 1));
	F d = .01,
	  k = 0.;
	for (F steps = 0.; steps < 80.; steps++) {
		p = v3(0, 0, -3) + u * d;
		v2 h = q(p);
		if (abs(h.x) < .004 * d) {
			k = h.y;
			break;
		}

		if (d > 5.) break;
		d += h.x;
	}

	if (k < .5) b = v3(stars(z));
	else {
		v3 y = v3(8. - 16. * iMouse.x / R.x, 6. - cos(iTime * .2), -1. - iMouse.y / 18.),
		   n = c(p),
		   o = v3(1.82, 1.8, 1.78) * dot(n, N(y - p));
		if (k > 1.5) {
			b = mix(mix(mix(v3(.05, .05, .8), v3(.05, .25, .05), S(.4, .52, fbm(n.xy * 3.1 + v2(iTime * .05, 0)))), v3(1), S(.8, .95, n.y) * S(.1, .8, fbm(n.xz * 10. + v2(iTime * .1, 0)))), v3(.3, .5, .95), S(-.5, 0., n.z));
			b *= o;
		}
		else if (k > .5) b = v3(.5) * o * pow(a(p, y), 2.);
	}

	j = vec4(pow(b, v3(.4545)), 1);
}