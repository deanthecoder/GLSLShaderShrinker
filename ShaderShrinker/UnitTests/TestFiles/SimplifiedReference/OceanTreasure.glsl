// Processed by 'GLSL Shader Shrinker' (Shrunk by 575 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define time	(iTime + 37.)

float smin(float a, float b, float k) {
	float h = clamp(.5 + .5 * (b - a) / k, 0., 1.);
	return mix(b, a, h) - k * h * (1. - h);
}

mat2 rot(float a) {
	float c = cos(a),
	      s = sin(a);
	return mat2(c, s, -s, c);
}

vec3 getRayDir(vec3 ro, vec2 uv) {
	vec3 forward = normalize(vec3(0, -3, 0) - ro),
	     right = normalize(cross(vec3(0, 1, 0), forward));
	return normalize(forward + right * uv.x + cross(forward, right) * uv.y);
}

vec2 min2(vec2 a, vec2 b) { return a.x < b.x ? a : b; }

float sdBox(vec3 p, vec3 b) {
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
}

float sdRod(vec3 p, float h, float r) {
	p.y -= clamp(p.y, 0., h);
	return length(p) - r;
}

float sdSurface(vec2 p) {
	float sh = texture(iChannel0, (p + vec2(0, 1) * (time + 1.)) * .05).r;
	sh -= texture(iChannel0, (p + vec2(.7, .2) * time) * .05).r;
	return clamp(.05 + sh * .2, 0., 1.);
}

float sdChest(vec3 p) {
	if (length(p) > 4.5) return 1e7;
	vec3 pp = p + vec3(0, .6, 0);
	float d,
	      box = sdBox(pp, vec3(1, .6, 1.5));
	box = max(box, -sdBox(pp - vec3(0, .9, 0), vec3(1, .6, 1.5)));
	p.xy *= mat2(.98007, .19867, -.19867, .98007);
	p.y -= .2;
	d = min(max(max(-p.y, length(p.xy) - 1.), abs(p.z) - 1.5), box) - texture(iChannel0, (p.xz + p.y) * .11).r * .1;
	d -= abs(abs(p.z) - .75) < .15 ? .07 : 0.;
	return d;
}

float sdFloor(vec3 p) {
	float bh = textureLod(iChannel0, p.xz * mat2(.4536, .89121, -.89121, .4536) * .01, 2.5).r * 6.5;
	bh += textureLod(iChannel0, (p.xz + vec2(12.3, 23.4)) * mat2(.87758, .47943, -.47943, .87758) * .02, 0.).r * 1.2;
	bh /= 2.5;
	return p.y + 6. - bh;
}

float sdBubble(vec3 p, float t) {
	float progress = pow(min(fract(t * .1) * 4.5, 1.), 2.);
	return length(p + vec3(1.2 - smoothstep(0., 1., min(progress * 5., 1.)) * .3, 4.2 * (1. - progress * progress), -1. + .2 * progress * sin(progress * 10.))) - mix(.01, .08, progress);
}

float sdPlant(vec3 p, float h) {
	float r = .02 * -(p.y + 2.5) - .005 * pow(sin(p.y * 30.), 2.);
	p.z += sin(time + h) * pow(.2 * (p.y + 5.6), 3.);
	return sdRod(p + vec3(0, 5.7, 0), 3. * h, r);
}

float sdPlants(vec3 p) {
	const vec3 dd = vec3(.2, 0, -.5);
	float d = 1e10;
	for (int i = 0; i < 4; i++) {
		d = min(d, min(sdPlant(p, 1.2), min(sdPlant(p + dd.xyx, .5), sdPlant(p + dd, .8))));
		p.x--;
		p.z--;
		p.xz *= mat2(.82534, .56464, -.56464, .82534);
	}

	return d;
}

float sdManta(vec3 p) {
	p.xz *= mat2(-1, 59e-5, -59e-5, -1);
	p.y += 3.5;
	p.z += 22.;
	float d,
	      t = mod(iTime, 20.);
	p.x -= 30.;
	p.xz *= rot(-t * .07);
	p.x += 30.;
	if (length(p) > 3.5) return 1e7;
	p.y -= sin(-time * 1.5) * .2;
	p.y -= (abs(p.x) + .1) * sin(abs(p.x) + time * 1.5) * .4;
	vec3 pp = p;
	pp.xz *= mat2(.70721, .707, -.707, .70721);
	d = smin(sdBox(pp, vec3(1, .015, 1)), length(p.xz * vec2(.5, 1)) - 1.18, -.05);
	pp = p;
	if (p.y > 0.) {
		pp.x = abs(pp.x) - .1;
		pp.z -= .6;
		d = smin(d, length(pp) - .1, .05);
	}

	p.z += 1.25;
	return (smin(d, sdBox(p, vec3(.005, .005, 2)), .3) - .02) * .7;
}

float godLight(vec3 p) {
	vec3 lightDir = normalize(vec3(1, 4, 3) - p),
	     sp = p + lightDir * -p.y;
	float f = 1. - clamp(sdSurface(sp.xz) * 10., 0., 1.);
	f *= 1. - length(lightDir.xz);
	return smoothstep(.2, 1., f * .7);
}

vec2 map(vec3 p) {
	vec3 pp = p;
	pp.xz *= mat2(.87758, -.47943, .47943, .87758);
	float surface = -p.y - sdSurface(p.xz),
	      t = time * .6;
	surface += (.5 + .5 * (sin(p.z * .2 + t) + sin((p.z + p.x) * .1 + t * 2.))) * .4;
	return min2(vec2(surface, 1.5), min2(vec2(sdChest(pp + vec3(2, 4.4, 0)), 2.5), min2(vec2(sdFloor(p), 3.5), min2(vec2(sdPlants(p - vec3(6, 0, 7)), 5.5), min2(vec2(sdManta(p), 6.5), min2(vec2(sdBubble(pp, time - .3), 4.5), vec2(sdBubble(pp, time), 4.5)))))));
}

vec3 calcNormal(vec3 p) {
	const vec2 e = vec2(1, -1) * .0025;
	return normalize(e.xyy * map(p + e.xyy).x + e.yyx * map(p + e.yyx).x + e.yxy * map(p + e.yxy).x + e.xxx * map(p + e.xxx).x);
}

float calcOcc(vec3 p, vec3 n) { return smoothstep(0., 1., 1. - (.5 - map(p + n * .5).x)); }

vec3 vignette(vec3 col, vec2 fragCoord) {
	vec2 q = fragCoord.xy / iResolution.xy;
	col *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return col;
}

float marchGodRay(vec3 ro, vec3 rd, float hitDist) {
	vec3 p = ro,
	     st = rd * hitDist / 96.;
	float god = 0.;
	for (int i = 0; i < 96; i++) {
		god += godLight(p);
		p += st;
	}

	god /= 96.;
	return smoothstep(0., 1., min(god, 1.));
}

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 uv = (fragCoord - .5 * iResolution.xy) / iResolution.y;
	vec3 rd, p, col,
	     ro = vec3(-.4, -2, -4);
	ro.xz *= rot(.03 * sin(time * .3));
	ro.y += sin(time * .2) * .3;
	rd = getRayDir(ro, uv);
	int hit = 0;
	float d = .01,
	      outside = 1.;
	for (float steps = 0.; steps < 1e2; steps++) {
		p = ro + rd * d;
		vec2 h = map(p);
		if (h.x < .001 * d) {
			if (h.y == 4.5) {
				rd = refract(rd, calcNormal(p) * sign(outside), 1.);
				outside *= -1.;
				continue;
			}

			hit = int(h.y);
			break;
		}

		if (d > 50.) break;
		d += h.x;
	}

	col = vec3(.002, .008, .02);
	if (hit > 0) {
		vec3 n = calcNormal(p),
		     mat = vec3(.15, .25, .6);
		if (hit == 1) n.y = -n.y;
		else {
			if (hit == 2) mat = mix(mat, vec3(.2, .15, .125), .5);
			else if (hit == 3) mat += vec3(.1, .1, 0);
			else if (hit == 5) mat += vec3(0, .2, 0);
			else if (hit == 6) mat += vec3(.5);
			mat *= .4 + .6 * godLight(p);
			mat *= calcOcc(p, n);
			vec3 lightDir = normalize(vec3(1, 4, 3) - p);
			float sha1 = max(0., map(p + lightDir * .25).x / .25),
			      sha2 = max(0., map(p + lightDir).x);
			mat *= clamp((sha1 + sha2) * .5, 0., 1.);
		}

		col = (.1 + max(0., dot(normalize(vec3(1, 4, 3) - p), n))) * mat;
	}

	fragColor = vec4(vignette(pow(mix(mix(col, vec3(.002, .008, .02), clamp(pow(d / 25., 1.5), 0., 1.)), vec3(1.8, 3, 3.6), marchGodRay(ro, rd, d)), vec3(.4545)), fragCoord), 1);
}