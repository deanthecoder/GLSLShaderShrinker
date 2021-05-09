const vec2 stepToStep = vec2(.36, .66);
const vec3 lightPos1 = vec3(0, 4.5, 4.3),
           lightPos2 = vec3(0, 4.5, -.6);

#define MY_GPU_CAN_TAKE_IT
#define ZERO	min(iTime, 0.)

float smin(float a, float b, float k) {
	float h = clamp(.5 + .5 * (b - a) / k, 0., 1.);
	return mix(b, a, h) - k * h * (1. - h);
}

mat2 rot(float a) {
	float c = cos(a),
	      s = sin(a);
	return mat2(c, s, -s, c);
}

float sdBox(vec3 p, vec3 b) {
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float sdCylinder(vec3 p, float h, float r) {
	vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(h, r);
	return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
	vec3 pa = p - a,
	     ba = b - a;
	return length(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0., 1.)) - r;
}

float sdLink(vec3 p, float le, float r1, float r2) {
	vec3 q = vec3(p.x, max(abs(p.y) - le, 0.), p.z);
	return length(vec2(length(q.xy) - r1, q.z)) - r2;
}

vec3 getRayDir(vec3 ro, vec3 lookAt, vec2 uv) {
	vec3 forward = normalize(lookAt - ro),
	     right = normalize(cross(vec3(0, 1, 0), forward));
	return normalize(forward + right * uv.x + cross(forward, right) * uv.y);
}

vec2 min2(vec2 a, vec2 b) { return a.x < b.x ? a : b; }

vec2 cappedMod(vec2 p, float c, vec2 l1, vec2 l2) { return p - c * clamp(round(p / c), -l1, l2); }

vec3 mirrorX(vec3 p, float d) {
	p.x = abs(p.x) - d;
	return p;
}

vec3 repeatXZ(vec3 p, float c, vec2 l1, vec2 l2) {
	p.xz = cappedMod(p.xz, c, l1, l2);
	return p;
}

float sdTactileSlab(vec3 p) {
	p.z -= .4;
	float d = sdBox(p, vec3(.385, .05, .4 - .015));
	p.y -= .01;
	p.xz = cappedMod(p.xz + vec2(.06666), .13332, vec2(2), vec2(3));
	return min(d, sdCylinder(p, .0225, .05));
}

float sdTactileSlabStrip(vec3 p) {
	p.xz = cappedMod(p.xz, .8, vec2(3, 0), vec2(4, 0));
	return sdTactileSlab(p);
}

float sdPavingSlab(vec3 p) { return sdBox(p, vec3(.485, .05, .5 - .015)); }

float sdThinPavingSlab(vec3 p) { return sdBox(p - vec3(0, 0, .2), vec3(.485, .05, .2 - .015)); }

float glow = 0.,
      flicker = 1.;
vec2 sdStep(vec3 p) {
	float d2,
	      d1 = sdBox(p, vec3(1.4, .02, .02));
	if (d1 > 1.) return vec2(d1, 3.5);
	p.y += .16 + .02;
	d2 = sdBox(p, vec3(1.385, .16 - .015, .02));
	d1 = min(d1, sdBox(p - vec3(0, 0, .015), vec3(1.4, .16, .02)));
	p.yz += vec2(.18, .32);
	return min2(vec2(min(d1, sdBox(p + vec3(0, .015, 0), vec3(1.4, .02, .32 - .015))), 3.5), vec2(min(d2, sdBox(p, vec3(1.385, .02, .32 - .015))), 4.5));
}

vec2 sdSteps(vec3 p) { return sdStep(p + max(0., floor(-p.z / stepToStep.y)) * vec3(0, stepToStep)); }

float sdRailHolder(vec3 p) {
	p.x -= .35;
	return max(sdLink(p.yxz, .1, .25, .06), p.y);
}

vec2 sdWalls(vec3 p) {
	vec3 pp,
	     op = p;
	p.yz = mod(p.yz, vec2(.16, .22));
	float d2,
	      d = sdBox(mirrorX(p, 2.8), vec3(.02, .16 - .015, .22 - .015)) - .015;
	pp = mirrorX(op, 2.8);
	pp.xy -= vec2(-.3, 1.3);
	d2 = 1e10;
	if (pp.y < 3.) {
		d2 = smin(sdCapsule(pp, vec3(0, 0, 1), vec3(0), .1), sdRailHolder(pp - vec3(0, 0, .75)), .025);
		pp.yz *= rot(-atan(stepToStep.x / stepToStep.y));
		d2 = smin(min(smin(d2, sdRailHolder(pp - vec3(0, 0, -.75)), .0255), sdCapsule(pp, vec3(0), vec3(0, 0, -length(stepToStep * 11.)), .1)), sdRailHolder(pp - vec3(0, 0, -length(stepToStep * 11.) + .75)), .025);
	}
	if (p.x > 0.) {
		const float nearEndZ = -stepToStep.y * 12.;
		float middleZ = mix(nearEndZ, 2.9, .5),
		      depthToInclude = 2.9 - nearEndZ;
		pp = op;
		pp.z -= middleZ;
		d = max(d, sdBox(pp, vec3(3, 1000, depthToInclude / 2.)));
	}
	return min2(vec2(d, 1.5), vec2(d2, 2.5));
}

vec2 sdCorridorSection(vec3 p) {
	vec2 d2,
	     d1 = sdWalls(p);
	if (p.y > 2.) return d1;
	d2 = sdSteps(repeatXZ(p, 2.8, vec2(1, 0), vec2(1, 0)));
	float d3,
	      d = sdThinPavingSlab(repeatXZ(p, 1., vec2(3, 0), vec2(3, 0)));
	p.z -= .4;
	d3 = sdTactileSlabStrip(p);
	d3 -= .006 * mix(.8, 1., texture(iChannel0, p.xz * 1.7).r);
	return min2(vec2(min(d, sdPavingSlab(repeatXZ(p - vec3(0, 0, .8 + .5), 1., vec2(3, 0), vec2(3, 5)))), 6.5), min2(d1, min2(d2, vec2(d3, 5.5))));
}

vec2 map(vec3 p, bool useGlow) {
	const vec3 v = vec3(1.8, 6. - 1.5, 0);
	float lightFrame, bump,
	      d = sdBox(p - vec3(2.7, 0, 1.8), vec3(.07, 10, .07)) - .01;
	d = min(min(min(d, sdBox(p - vec3(0, 6. - 1., 1.8), vec3(2.8, 1, .07)) - .01), sdCapsule(p, v + vec3(0, 0, 2.9), v - vec3(0, 0, 100), .18)), sdCapsule(p, v + vec3(0, 0, 2), v + vec3(0, 0, 1.6), .22));
	vec3 pp = p;
	pp.z = abs(p.z - 1.8);
	d = min(min(d, sdLink(pp - vec3(1.8, 6. - 1.3, .9), .2, .2, .015)), sdLink(p - vec3(1.8, 6. - 1.3, -2), .2, .2, .015));
	pp -= vec3(0, 6. - 1., 2.5);
	lightFrame = max(sdBox(pp, vec3(1, .25, .06)), -sdBox(pp + vec3(0, .3, 0), vec3(.95, .2, .1)));
	d = min(d, lightFrame);
	if (useGlow) {
		float gd,
		      endFade = 1. - clamp((abs(p.x) - .8) / .2, 0., 1.);
		pp.y += .2;
		gd = sdCylinder(pp.yxz, .05, .92);
		glow += endFade * .001 / (.001 + gd * gd * .3) * mix(.01, 1., p.z < 0. ? 1. : flicker);
	}
	pp = p;
	pp.xz *= rot(-.78538);
	bump = texture(iChannel0, p.xz * .8).r * .01;
	d = min(d, sdBox(pp - vec3(0, 6, -20), vec3(10, 1, 20. + stepToStep * 12.)) - bump);
	vec2 d2 = sdCorridorSection(p);
	p.yz -= stepToStep * 12.;
	p.z -= 9.;
	p.xz *= rot(-.7854);
	p.x -= 9.08;
	return min2(vec2(d, 1.5), min2(d2, sdCorridorSection(p)));
}

vec3 calcNormal(vec3 p) {
	const vec2 e = vec2(1, -1) * .5773e-4;
	return normalize(e.xyy * map(p + e.xyy, false).x + e.yyx * map(p + e.yyx, false).x + e.yxy * map(p + e.yxy, false).x + e.xxx * map(p + e.xxx, false).x);
}

float calcAO(vec3 p, vec3 n, float d) { return clamp(map(p + n * d, false).x / d, 0., 1.); }

float calcShadow(vec3 p, vec3 lightPos) {
	float std,
	      d = distance(p, lightPos),
	      shadow = 1.;
	vec3 st = (lightPos - p) / 37.5;
	std = length(st);
	p += normalize(lightPos - p) * .01;
	for (float i = ZERO; i < 30.; i++) {
		p += st;
		shadow = min(shadow, max(map(p, false).x, 0.) / (std * i));
	}
	return shadow / pow(d / 20. + 1., 2.);
}

vec3 vignette(vec3 col, vec2 fragCoord) {
	vec2 q = fragCoord.xy / iResolution.xy;
	col *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return col;
}

vec3 getMaterial(vec3 p, vec3 rd, vec3 n, float id) {
	vec3 mat,
	     lightDir1 = normalize(lightPos1 - p),
	     lightDir2 = normalize(lightPos2 - p);
	float diff, occ, sha,
	      spec = pow(max(max(dot(reflect(lightDir1, n), rd) * flicker, dot(reflect(lightDir2, n), rd)), 0.), 50.);
	if (id == 3.5) mat = vec3(.1);
	else if (id == 4.5) mat = vec3(smoothstep(0., .6, texture(iChannel0, (abs(n.y) < .1 ? p.xy : p.xz) * 1.4125).r));
	else if (id == 5.5) {
		mat = vec3(.9, .75, .21);
		spec *= .8;
	}
	else if (id == 6.5) mat = vec3(mix(.3, .5, texture(iChannel0, p.xz * 1.743).r));
	else mat = vec3(1);
	diff = max(max(0., dot(lightDir1, n) * flicker), dot(lightDir2, n));
	occ = min(1., .2 + calcAO(p, n, .15) * calcAO(p, n, .05));
	sha = (calcShadow(p, lightPos1) * flicker + calcShadow(p, lightPos2)) / 2.;
	return mat * vec3(1, 1, 1.1) * ((diff + spec) * sha + occ * .025) + min(glow, 1.);
}

void march(vec3 ro, vec3 rd, out vec3 p, out vec2 h) {
	float d = .01;
	for (float steps = ZERO; steps < 60.; steps++) {
		p = ro + rd * d;
		h = map(p, true);
		if (abs(h.x) < .0015 * d) break;
		d += h.x * .9;
	}
}

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 h,
	     uv = (fragCoord - .5 * iResolution.xy) / iResolution.y;
	flicker = step(.25, sin(iTime) * texture(iChannel0, vec2(iTime * .1)).r);
	float ft = fract(iTime / 5.),
	      phase = mod(floor(iTime / 5.), 3.);
	vec3 ro, rd, p, n, col,
	     lookAt = vec3(0, 1, 0);
	if (phase == 0.) ro = vec3(mix(0., -.5, ft) * -3. - 1., -1. - 6. * mix(.5, .4, ft), -10);
	else if (phase == 1.) {
		ro = vec3(-3. * mix(0., .5, ft) - 1., 3, -4);
		lookAt = lightPos2;
	}
	else if (phase == 2.) {
		ro = vec3(.5, -1. - 6. * (mix(.25, 0., ft) - .5), -1);
		lookAt = lightPos1 - mix(vec3(5, 3.5, -1), vec3(0), ft);
	}
	rd = getRayDir(ro, lookAt, uv);
	march(ro, rd, p, h);
	n = calcNormal(p);
	col = getMaterial(p, rd, n, h.y);
#ifdef MY_GPU_CAN_TAKE_IT
	if (h.y == 2.5) {
		rd = reflect(rd, n);
		march(p, rd, p, h);
		col = mix(col, getMaterial(p, rd, n, h.y), .75);
	}
#endif
	col *= exp(-pow(distance(ro, p) / 30., 3.) * 5.);
	col = vignette(pow(col, vec3(.4545)), fragCoord);
	fragColor = vec4(col, 1);
}