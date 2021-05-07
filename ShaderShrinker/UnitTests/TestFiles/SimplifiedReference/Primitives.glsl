#if HW_PERFORMANCE == 0
#define AA	1

#else
#define AA	2 // make this 2 or 3 for antialiasing

#endif
float dot2(vec2 v) { return dot(v, v); }

float dot2(vec3 v) { return dot(v, v); }

float ndot(vec2 a, vec2 b) { return a.x * b.x - a.y * b.y; }

float sdSphere(vec3 p, float s) { return length(p) - s; }

float sdBox(vec3 p, vec3 b) {
	vec3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.));
}

float sdBoundingBox(vec3 p, vec3 b, float e) {
	p = abs(p) - b;
	vec3 q = abs(p + e) - e;
	return min(min(length(max(vec3(p.x, q.yz), 0.)) + min(max(p.x, max(q.y, q.z)), 0.), length(max(vec3(q.x, p.y, q.z), 0.)) + min(max(q.x, max(p.y, q.z)), 0.)), length(max(vec3(q.xy, p.z), 0.)) + min(max(q.x, max(q.y, p.z)), 0.));
}

float sdEllipsoid(vec3 p, vec3 r) {
	float k0 = length(p / r);
	return k0 * (k0 - 1.) / length(p / (r * r));
}

float sdTorus(vec3 p, vec2 t) { return length(vec2(length(p.xz) - t.x, p.y)) - t.y; }

float sdCappedTorus(vec3 p, vec2 sc, float ra, float rb) {
	p.x = abs(p.x);
	return sqrt(dot(p, p) + ra * ra - 2. * ra * ((sc.y * p.x > sc.x * p.y) ? dot(p.xy, sc) : length(p.xy))) - rb;
}

float sdHexPrism(vec3 p, vec2 h) {
	const vec3 k = vec3(-.8660254, .5, .57735);
	p = abs(p);
	p.xy -= 2. * min(dot(k.xy, p.xy), 0.) * k.xy;
	vec2 d = vec2(length(p.xy - vec2(clamp(p.x, -k.z * h.x, k.z * h.x), h.x)) * sign(p.y - h.x), p.z - h.y);
	return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdOctogonPrism(vec3 p, float r, float h) {
	const vec3 k = vec3(-.9238795325, .3826834323, .4142135623);
	p = abs(p);
	p.xy -= 2. * min(dot(k.xy, p.xy), 0.) * k.xy;
	p.xy -= 2. * min(dot(vec2(-k.x, k.y), p.xy), 0.) * vec2(-k.x, k.y);
	p.xy -= vec2(clamp(p.x, -k.z * r, k.z * r), r);
	vec2 d = vec2(length(p.xy) * sign(p.y), p.z - h);
	return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
	vec3 pa = p - a,
	     ba = b - a;
	return length(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0., 1.)) - r;
}

float sdRoundCone(vec3 p, float r1, float r2, float h) {
	vec2 q = vec2(length(p.xz), p.y);
	float b = (r1 - r2) / h,
	      a = sqrt(1. - b * b),
	      k = dot(q, vec2(-b, a));
	if (k < 0.) return length(q) - r1;
	if (k > a * h) return length(q - vec2(0, h)) - r2;
	return dot(q, vec2(a, b)) - r1;
}

float sdRoundCone(vec3 p, vec3 a, vec3 b, float r1, float r2) {
	vec3 pa,
	     ba = b - a;
	float y, z, x2, y2, z2, k,
	      l2 = dot(ba, ba),
	      rr = r1 - r2,
	      a2 = l2 - rr * rr,
	      il2 = 1. / l2;
	pa = p - a;
	y = dot(pa, ba);
	z = y - l2;
	x2 = dot2(pa * l2 - ba * y);
	y2 = y * y * l2;
	z2 = z * z * l2;
	k = sign(rr) * rr * rr * x2;
	if (sign(z) * a2 * z2 > k) return sqrt(x2 + z2) * il2 - r2;
	if (sign(y) * a2 * y2 < k) return sqrt(x2 + y2) * il2 - r1;
	return (sqrt(x2 * a2 * il2) + y * rr) * il2 - r1;
}

float sdTriPrism(vec3 p, vec2 h) {
	const float k = sqrt(3.);
	h.x *= .5 * k;
	p.xy /= h.x;
	p.x = abs(p.x) - 1.;
	p.y += 1. / k;
	if (p.x + k * p.y > 0.) p.xy = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.;
	p.x -= clamp(p.x, -2., 0.);
	float d1 = length(p.xy) * sign(-p.y) * h.x,
	      d2 = abs(p.z) - h.y;
	return length(max(vec2(d1, d2), 0.)) + min(max(d1, d2), 0.);
}

float sdCylinder(vec3 p, vec2 h) {
	vec2 d = abs(vec2(length(p.xz), p.y)) - h;
	return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdCylinder(vec3 p, vec3 a, vec3 b, float r) {
	vec3 pa = p - a,
	     ba = b - a;
	float baba = dot(ba, ba),
	      paba = dot(pa, ba),
	      x = length(pa * baba - ba * paba) - r * baba,
	      y = abs(paba - baba * .5) - baba * .5,
	      x2 = x * x,
	      y2 = y * y * baba,
	      d = (max(x, y) < 0.) ? -min(x2, y2) : (((x > 0.) ? x2 : 0.) + ((y > 0.) ? y2 : 0.));
	return sign(d) * sqrt(abs(d)) / baba;
}

float sdCone(vec3 p, vec2 c, float h) {
	vec2 q = h * vec2(c.x, -c.y) / c.y,
	     w = vec2(length(p.xz), p.y),
	     a = w - q * clamp(dot(w, q) / dot(q, q), 0., 1.),
	     b = w - q * vec2(clamp(w.x / q.x, 0., 1.), 1);
	float k = sign(q.y);
	return sqrt(min(dot(a, a), dot(b, b))) * sign(max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y)));
}

float sdCappedCone(vec3 p, float h, float r1, float r2) {
	vec2 q = vec2(length(p.xz), p.y),
	     k1 = vec2(r2, h),
	     k2 = vec2(r2 - r1, 2. * h),
	     ca = vec2(q.x - min(q.x, (q.y < 0.) ? r1 : r2), abs(q.y) - h),
	     cb = q - k1 + k2 * clamp(dot(k1 - q, k2) / dot2(k2), 0., 1.);
	return ((cb.x < 0. && ca.y < 0.) ? -1. : 1.) * sqrt(min(dot2(ca), dot2(cb)));
}

float sdCappedCone(vec3 p, vec3 a, vec3 b, float ra, float rb) {
	float rba = rb - ra,
	      baba = dot(b - a, b - a),
	      papa = dot(p - a, p - a),
	      paba = dot(p - a, b - a) / baba,
	      x = sqrt(papa - paba * paba * baba),
	      cax = max(0., x - ((paba < .5) ? ra : rb)),
	      cay = abs(paba - .5) - .5,
	      k = rba * rba + baba,
	      f = clamp((rba * (x - ra) + paba * baba) / k, 0., 1.),
	      cbx = x - ra - f * rba,
	      cby = paba - f;
	return ((cbx < 0. && cay < 0.) ? -1. : 1.) * sqrt(min(cax * cax + cay * cay * baba, cbx * cbx + cby * cby * baba));
}

float sdSolidAngle(vec3 pos, vec2 c, float ra) {
	vec2 p = vec2(length(pos.xz), pos.y);
	return max((length(p) - ra), length(p - c * clamp(dot(p, c), 0., ra)) * sign(c.y * p.x - c.x * p.y));
}

float sdOctahedron(vec3 p, float s) {
	p = abs(p);
	float m = p.x + p.y + p.z - s;
	vec3 q;
	if (3. * p.x < m) q = p.xyz;
	else if (3. * p.y < m) q = p.yzx;
	else if (3. * p.z < m) q = p.zxy;
	else return m * .57735027;
	float k = clamp(.5 * (q.z - q.y + s), 0., s);
	return length(vec3(q.x, q.y - s + k, q.z - k));
}

float sdPyramid(vec3 p, float h) {
	float s, t,
	      m2 = h * h + .25;
	p.xz = abs(p.xz);
	p.xz = (p.z > p.x) ? p.zx : p.xz;
	p.xz -= .5;
	vec3 q = vec3(p.z, h * p.y - .5 * p.x, h * p.x + .5 * p.y);
	s = max(-q.x, 0.);
	t = clamp((q.y - .5 * p.z) / (m2 + .25), 0., 1.);
	return sqrt(((min(q.y, -q.x * m2 - q.y * .5) > 0. ? 0. : min((m2 * (q.x + s) * (q.x + s) + q.y * q.y), (m2 * (q.x + .5 * t) * (q.x + .5 * t) + (q.y - m2 * t) * (q.y - m2 * t)))) + q.z * q.z) / m2) * sign(max(q.z, -p.y));
}

float sdRhombus(vec3 p, float la, float lb, float h, float ra) {
	p = abs(p);
	vec2 q,
	     b = vec2(la, lb);
	float f = clamp(ndot(b, b - 2. * p.xz) / dot(b, b), -1., 1.);
	q = vec2(length(p.xz - .5 * b * vec2(1. - f, 1. + f)) * sign(p.x * b.y + p.z * b.x - b.x * b.y) - ra, p.y - h);
	return min(max(q.x, q.y), 0.) + length(max(q, 0.));
}

vec2 opU(vec2 d1, vec2 d2) { return (d1.x < d2.x) ? d1 : d2; }

#define ZERO	(min(iFrame, 0))

vec2 map(vec3 pos) {
	vec2 res = vec2(1e10, 0);
	{ res = opU(res, vec2(sdSphere(pos - vec3(-2, .25, 0), .25), 26.9)); }
	if (sdBox(pos - vec3(0, .3, -1), vec3(.35, .3, 2.5)) < res.x) res = opU(opU(opU(opU(opU(res, vec2(sdBoundingBox(pos - vec3(0, .25, 0), vec3(.3, .25, .2), .025), 16.9)), vec2(sdTorus((pos - vec3(0, .3, 1)).xzy, vec2(.25, .05)), 25)), vec2(sdCone(pos - vec3(0, .45, -1), vec2(.6, .8), .45), 55)), vec2(sdCappedCone(pos - vec3(0, .25, -2), .25, .25, .1), 13.67)), vec2(sdSolidAngle(pos - vec3(0, 0, -3), vec2(3, 4) / 5., .4), 49.13));
	if (sdBox(pos - vec3(1, .3, -1), vec3(.35, .3, 2.5)) < res.x) res = opU(opU(opU(opU(opU(res, vec2(sdCappedTorus((pos - vec3(1, .3, 1)) * vec3(1, -1, 1), vec2(.866025, -.5), .25, .05), 8.5)), vec2(sdBox(pos - vec3(1, .25, 0), vec3(.3, .25, .1)), 3)), vec2(sdCapsule(pos - vec3(1, 0, -1), vec3(-.1, .1, -.1), vec3(.2, .4, .2), .1), 31.9)), vec2(sdCylinder(pos - vec3(1, .25, -2), vec2(.15, .25)), 8)), vec2(sdHexPrism(pos - vec3(1, .2, -3), vec2(.2, .05)), 18.4));
	if (sdBox(pos - vec3(-1, .35, -1), vec3(.35, .35, 2.5)) < res.x) res = opU(opU(opU(opU(opU(res, vec2(sdPyramid(pos - vec3(-1, -.6, -3), 1.), 13.56)), vec2(sdOctahedron(pos - vec3(-1, .15, -2), .35), 23.56)), vec2(sdTriPrism(pos - vec3(-1, .15, -1), vec2(.3, .05)), 43.5)), vec2(sdEllipsoid(pos - vec3(-1, .25, 0), vec3(.2, .25, .05)), 43.17)), vec2(sdRhombus((pos - vec3(-1, .34, 1)).xzy, .15, .25, .04, .08), 17));
	if (sdBox(pos - vec3(2, .3, -1), vec3(.35, .3, 2.5)) < res.x) res = opU(opU(opU(opU(opU(res, vec2(sdOctogonPrism(pos - vec3(2, .2, -3), .2, .05), 51.8)), vec2(sdCylinder(pos - vec3(2, .15, -2), vec3(.1, -.1, 0), vec3(-.2, .35, .1), .08), 31.2)), vec2(sdCappedCone(pos - vec3(2, .1, -1), vec3(.1, 0, 0), vec3(-.2, .4, .1), .15, .05), 46.1)), vec2(sdRoundCone(pos - vec3(2, .15, 0), vec3(.1, 0, 0), vec3(-.1, .35, .1), .15, .05), 51.7)), vec2(sdRoundCone(pos - vec3(2, .2, 1), .2, .1, .3), 37));
	return res;
}

vec2 iBox(vec3 ro, vec3 rd, vec3 rad) {
	vec3 m = 1. / rd,
	     n = m * ro,
	     k = abs(m) * rad,
	     t1 = -n - k,
	     t2 = -n + k;
	return vec2(max(max(t1.x, t1.y), t1.z), min(min(t2.x, t2.y), t2.z));
}

vec2 raycast(vec3 ro, vec3 rd) {
	vec2 tb,
	     res = vec2(-1);
	float tmin = 1.,
	      tmax = 20.,
	      tp1 = (0. - ro.y) / rd.y;
	if (tp1 > 0.) {
		tmax = min(tmax, tp1);
		res = vec2(tp1, 1);
	}
	tb = iBox(ro - vec3(0, .4, -.5), rd, vec3(2.5, .41, 3));
	if (tb.x < tb.y && tb.y > 0. && tb.x < tmax) {
		tmin = max(tb.x, tmin);
		tmax = min(tb.y, tmax);
		float t = tmin;
		for (int i = 0; i < 70 && t < tmax; i++) {
			vec2 h = map(ro + rd * t);
			if (abs(h.x) < .0001 * t) {
				res = vec2(t, h.y);
				break;
			}
			t += h.x;
		}
	}
	return res;
}

float calcSoftshadow(vec3 ro, vec3 rd, float mint, float tmax) {
	float res, t,
	      tp = (.8 - ro.y) / rd.y;
	if (tp > 0.) tmax = min(tmax, tp);
	res = 1.;
	t = mint;
	for (int i = ZERO; i < 24; i++) {
		float h = map(ro + rd * t).x,
		      s = clamp(8. * h / t, 0., 1.);
		res = min(res, s * s * (3. - 2. * s));
		t += clamp(h, .02, .2);
		if (res < .004 || t > tmax) break;
	}
	return clamp(res, 0., 1.);
}

vec3 calcNormal(vec3 pos) {
	vec3 n = vec3(0);
	for (int i = ZERO; i < 4; i++) {
		vec3 e = .5773 * (2. * vec3((((i + 3) >> 1) & 1), ((i >> 1) & 1), (i & 1)) - 1.);
		n += e * map(pos + .0005 * e).x;
	}
	return normalize(n);
}

float calcAO(vec3 pos, vec3 nor) {
	float occ = 0.,
	      sca = 1.;
	for (int i = ZERO; i < 5; i++) {
		float h = .01 + .12 * float(i) / 4.,
		      d = map(pos + h * nor).x;
		occ += (h - d) * sca;
		sca *= .95;
		if (occ > .35) break;
	}
	return clamp(1. - 3. * occ, 0., 1.) * (.5 + .5 * nor.y);
}

float checkersGradBox(vec2 p, vec2 dpdx, vec2 dpdy) {
	vec2 w = abs(dpdx) + abs(dpdy) + .001,
	     i = 2. * (abs(fract((p - .5 * w) * .5) - .5) - abs(fract((p + .5 * w) * .5) - .5)) / w;
	return .5 - .5 * i.x * i.y;
}

vec3 render(vec3 ro, vec3 rd, vec3 rdx, vec3 rdy) {
	vec3 col = vec3(.7, .7, .9) - max(rd.y, 0.) * .3;
	vec2 res = raycast(ro, rd);
	float t = res.x,
	      m = res.y;
	if (m > -.5) {
		vec3 lin,
		     pos = ro + t * rd,
		     nor = (m < 1.5) ? vec3(0, 1, 0) : calcNormal(pos),
		     ref = reflect(rd, nor);
		col = .2 + .2 * sin(m * 2. + vec3(0, 1, 2));
		float occ,
		      ks = 1.;
		if (m < 1.5) {
			vec3 dpdx = ro.y * (rd / rd.y - rdx / rdx.y),
			     dpdy = ro.y * (rd / rd.y - rdy / rdy.y);
			float f = checkersGradBox(3. * pos.xz, 3. * dpdx.xz, 3. * dpdy.xz);
			col = .15 + f * vec3(.05);
			ks = .4;
		}
		occ = calcAO(pos, nor);
		lin = vec3(0);
		{
			vec3 lig = normalize(vec3(-.5, .4, -.6)),
			     hal = normalize(lig - rd);
			float spe,
			      dif = clamp(dot(nor, lig), 0., 1.);
			dif *= calcSoftshadow(pos, lig, .02, 2.5);
			spe = pow(clamp(dot(nor, hal), 0., 1.), 16.);
			spe *= dif;
			spe *= .04 + .96 * pow(clamp(1. - dot(hal, lig), 0., 1.), 5.);
			lin += col * 2.2 * dif * vec3(1.3, 1, .7);
			lin += 5. * spe * vec3(1.3, 1, .7) * ks;
		}
		{
			float spe,
			      dif = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));
			dif *= occ;
			spe = smoothstep(-.2, .2, ref.y);
			spe *= dif;
			spe *= .04 + .96 * pow(clamp(1. + dot(nor, rd), 0., 1.), 5.);
			spe *= calcSoftshadow(pos, ref, .02, 2.5);
			lin += col * .6 * dif * vec3(.4, .6, 1.15);
			lin += 2. * spe * vec3(.4, .6, 1.3) * ks;
		}
		{
			float dif = clamp(dot(nor, normalize(vec3(.5, 0, .6))), 0., 1.) * clamp(1. - pos.y, 0., 1.);
			dif *= occ;
			lin += col * .55 * dif * vec3(.25);
		}
		{
			float dif = pow(clamp(1. + dot(nor, rd), 0., 1.), 2.);
			dif *= occ;
			lin += col * .25 * dif * vec3(1);
		}
		col = mix(lin, vec3(.7, .7, .9), 1. - exp(-.0001 * t * t * t));
	}
	return vec3(clamp(col, 0., 1.));
}

mat3 setCamera(vec3 ro, vec3 ta, float cr) {
	vec3 cw = normalize(ta - ro),
	     cp = vec3(sin(cr), cos(cr), 0),
	     cu = normalize(cross(cw, cp));
	return mat3(cu, cross(cu, cw), cw);
}

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	const vec3 ta = vec3(.5, -.5, -.6);
	vec2 mo = iMouse.xy / iResolution.xy;
	float time = 32. + iTime * 1.5;
	vec3 ro = ta + vec3(4.5 * cos(.1 * time + 7. * mo.x), 1.3 + 2. * mo.y, 4.5 * sin(.1 * time + 7. * mo.x)),
	     tot = vec3(0);
	mat3 ca = setCamera(ro, ta, 0.);
#if AA > 1
	for (int m = ZERO; m < AA; m++) {
		for (int n = ZERO; n < AA; n++) {
			vec2 o = vec2(float(m), float(n)) / float(AA) - .5;
			vec2 p = (2. * (fragCoord + o) - iResolution.xy) / iResolution.y;
#else
			vec2 p = (2. * fragCoord - iResolution.xy) / iResolution.y;
#endif
			vec3 rd = ca * normalize(vec3(p, 2.5));
			vec2 px = (2. * (fragCoord + vec2(1, 0)) - iResolution.xy) / iResolution.y;
			vec2 py = (2. * (fragCoord + vec2(0, 1)) - iResolution.xy) / iResolution.y;
			vec3 rdx = ca * normalize(vec3(px, 2.5));
			vec3 rdy = ca * normalize(vec3(py, 2.5));
			vec3 col = render(ro, rd, rdx, rdy);
			col = pow(col, vec3(.4545));
			tot += col;
#if AA > 1
		}
	}
	tot /= float(AA * AA);
#endif
	fragColor = vec4(tot, 1);
}