// Processed by 'GLSL Shader Shrinker' (Shrunk by 868 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define FAST_NOISE

float hash(vec2 p) { return fract(sin(dot(p, vec2(123.45, 87.43))) * 5432.3); }

float noise(vec2 p) {
#ifdef FAST_NOISE
	p *= .05;
	return texture(iChannel1, p).r;
#else
	vec2 i = floor(p);
	vec2 f = fract(p);
	float a = hash(i);
	float b = hash(i + vec2(1, 0));
	return mix(a, b, f.x) + (hash(i + vec2(0, 1)) - a) * f.y * (1. - f.x) + (hash(i + vec2(1)) - b) * f.x * f.y;
#endif
}

float fbm(vec2 p) {
	float f = .5 * noise(p * 1.1);
	f += .22 * noise(p * 2.3);
	f += .0625 * noise(p * 8.4);
	return f / .7825;
}

float smin(float a, float b, float k) {
	float h = clamp(.5 + .5 * (b - a) / k, 0., 1.);
	return mix(b, a, h) - k * h * (1. - h);
}

mat2 rot(float a) {
	float c = cos(a),
	      s = sin(a);
	return mat2(c, s, -s, c);
}

vec2 min2(vec2 a, vec2 b) { return a.x < b.x ? a : b; }

float sdTorus(vec3 p, vec2 t) { return length(vec2(length(p.xy) - t.x, p.z)) - t.y; }

float sdCorridorRibs(vec3 p) {
	p.z = mod(p.z, .6) - .3;
	return sdTorus(p, vec2(2, .2));
}

float sdCorridorTube(vec3 p) {
	float lxy = length(p.xy);
	return max(lxy - 2., 1.9 - lxy) - fbm((p.xy + p.yz) * 4.) * .05;
}

float sdCorridorRoof(vec3 p) {
	p.y -= 2.5;
	return length(p.xy) - 1.2 + .2 * pow(abs(.5 + .5 * sin(p.z * 1.4)), 4.);
}

vec3 applyCorriderCurve(vec3 p) {
	p.xz *= rot(-.01 * p.z);
	if (iTime < 44.) p.z += iTime;
	else p.z += 44. + 2.6 * smoothstep(0., 1., min(1., (iTime - 44.) / 2.6));

	return p;
}

float sdCorridorFloor(vec3 p) {
	p = applyCorriderCurve(p);
	p.y += sin(p.x * 1.1 + sin(p.z * .7)) * .15;
	return min(p.y + 1.5 + sin(p.z) * .05, length(p - vec3(0, -3.3, 49)) - 2.) - fbm(p.xz) * .1;
}

float sdCorridor(vec3 p) {
	p = applyCorriderCurve(p);
	vec3 pp = p;
	pp.x = abs(pp.x) - .8;
	return smin(smin(sdCorridorRibs(pp), sdCorridorTube(pp), .1), sdCorridorRoof(p), .3);
}

float sdEgg(vec3 p) {
	p = applyCorriderCurve(p);
	vec3 pp = p;
	pp.z -= 49.;
	pp.y++;
	pp.y *= .7;
	float d = length(pp) - .4,
	      openness = min(1., max(0., iTime - 45.) * .1);
	if (iTime >= 55.) openness += sin(iTime - 55.) * .05;
	d = smin(smin(d, -(length(pp.xz) - p.y * .5 - mix(.1, .7, openness)), -.1), sdTorus((pp - vec3(0, mix(.4, .25, openness), 0)).xzy, vec2(.35, .04) * openness), .05 * openness);
	d -= fbm(pp.xz + pp.xy) * .05;
	return d;
}

float sdSurprise(vec3 p) {
	float t = min(1., max(0., iTime - 62.));
	if (t <= 0.) return 1e10;
	p = applyCorriderCurve(p);
	p.z -= 49.;
	p.y += 1.5 - 1.4 * sin(t * 1.57079);
	return length(p) - mix(.1, 2., clamp(0., 1., max(0., iTime - 62.8) * 2.5));
}

vec3 getRayDir(vec3 ro, vec3 lookAt, vec2 uv) {
	vec3 forward = normalize(lookAt - ro),
	     right = normalize(cross(vec3(0, 1, 0), forward));
	return normalize(forward + right * uv.x + cross(forward, right) * uv.y);
}

vec2 map(vec3 p) { return min2(min2(min2(vec2(sdCorridor(p), 1.5), vec2(sdCorridorFloor(p), 2.5)), vec2(sdEgg(p), 3.5)), vec2(sdSurprise(p), 4.5)); }

vec3 calcNormal(vec3 p) {
	const vec2 e = vec2(1, -1) * 29e-5;
	return normalize(e.xyy * map(p + e.xyy).x + e.yyx * map(p + e.yyx).x + e.yxy * map(p + e.yxy).x + e.xxx * map(p + e.xxx).x);
}

float calcShadow(vec3 p) {
	vec3 rd = normalize(vec3(0, -.75, 0) - p);
	float h,
	      minH = 1.,
	      d = .01;
	for (int i = 0; i < 16; i++) {
		h = map(p + rd * d).x;
		minH = abs(h / d);
		if (minH < .01) return 0.;
		d += h;
	}

	return minH * 5.;
}

float calcOcc(vec3 p, vec3 n) { return 1. - (.3 - map(p + n * .3).x) * 4.; }

float calcSpotlight(vec3 p, vec3 lightDir) {
	float cutOff = .1,
	      edgeBlur = .02,
	      l = dot(normalize(vec3(0, -.75, 0) - p), -lightDir);
	edgeBlur++;
	float spotLight = smoothstep(1. - cutOff, (1. - cutOff) * edgeBlur, l) * .3;
	cutOff *= .7;
	spotLight = max(spotLight, smoothstep(1. - cutOff, (1. - cutOff) * 1.06, l)) * .5;
	cutOff *= .7;
	return max(spotLight, smoothstep(1. - cutOff, (1. - cutOff) * 1.07, l));
}

vec3 vignette(vec3 col, vec2 fragCoord) {
	vec2 q = fragCoord.xy / iResolution.xy;
	col *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return col;
}

vec2 getTorchDir() {
	float tSeg = iTime;
	if (tSeg < 2.) return vec2(0, 1);
	tSeg -= 2.;
	if (tSeg < 4.) return vec2(0, mix(1., 0., smoothstep(0., 1., min(1., tSeg / 2.))));
	tSeg -= 4.;
	if (tSeg < 3.5) return vec2(0, mix(0., -.4, smoothstep(0., 1., min(1., tSeg / 1.5))));
	tSeg -= 3.5;
	if (tSeg < 4.) {
		float f = smoothstep(0., 1., min(1., tSeg / 4.));
		return vec2(sin(f * 3.141) * -.6, -.4 + 1.1 * sin(f * 1.5705));
	}

	tSeg -= 4.;
	if (tSeg < 12.) return vec2(0, mix(.7, -.2, smoothstep(0., 1., min(1., tSeg))));
	tSeg -= 12.;
	if (tSeg < 17.) return vec2(0, mix(-.2, -.05, smoothstep(0., 1., min(1., tSeg / 5.))));
	tSeg -= 17.;
	return vec2(0, mix(-.05, -.35, smoothstep(0., 1., min(1., tSeg / 5.))));
}

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 uv = (fragCoord - .5 * iResolution.xy) / iResolution.y;
#ifdef USE_WEBCAM
	if (iTime > 63.5) {
		fragColor = vec4(mix(vec3(0), texture(iChannel0, fragCoord / iResolution.xy).rgb, min(1., (iTime - 63.5) * 5.)), 1);
		return;
	}

#endif
	vec3 ro, rd, p, col,
	     torchDir = normalize(vec3(getTorchDir(), .8));
	vec2 walkBump = vec2(sin(iTime * 2.5) * .05, pow(.5 + .5 * sin(iTime * 5.), 2.) * .03);
	walkBump *= mix(1., 0., min(1., max(0., iTime - 44.) / 2.6));
	ro = vec3(walkBump, 0);
	rd = getRayDir(ro, torchDir + vec3(0, .1, 0), uv);
	int hit = 0;
	float d = .01;
	for (float steps = 0.; steps < 120.; steps++) {
		p = ro + rd * d;
		vec2 h = map(p);
		if (h.x < .005 * d) {
			hit = int(h.y);
			break;
		}

		d += h.x;
	}

	if (hit > 0) {
		vec3 mat,
		     n = calcNormal(p),
		     lightToPoint = normalize(vec3(0, -.75, 0) - p);
		float sha = calcShadow(p),
		      occ = calcOcc(p, n),
		      spe = pow(max(0., dot(rd, reflect(lightToPoint, n))), 3.),
		      torch = calcSpotlight(p, torchDir),
		      backLight = clamp(dot(n, -rd), .01, 1.) * .05,
		      fog = 1. - exp(-d * .006);
		if (hit == 1) mat = vec3(.05, .06, .05);
		else if (hit == 2) mat = vec3(.033, .036, .036);
		else if (hit == 3) mat = mix(vec3(.5, .3, .2), vec3(.05, .06, .05), .7);
		else if (hit == 4) mat = vec3(0);

		col = (torch * sha + (backLight + spe) * occ) * vec3(1, .9, .8);
		col *= mat;
		col += torch * .02 * vec3(1, .9, .8);
		col = mix(col, vec3(.15, .2, .25), fog);
	}

	fragColor = vec4(pow(vignette(col, fragCoord), vec3(.4545)), 1);
}