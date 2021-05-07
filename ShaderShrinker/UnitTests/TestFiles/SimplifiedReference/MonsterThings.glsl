const vec2 e = vec2(.01, -.01);
vec2 z;
float t, tt, g, gg;
vec3 sp, tp, ep, op, hp, po, no, ld, al;
float smin(float a, float b, float h) {
	float k = clamp((a - b) / h * .5 + .5, 0., 1.);
	return mix(a, b, k) - k * (1. - k) * h;
}

float smax(float d1, float d2, float k) {
	float h = clamp(.5 - .5 * (d2 + d1) / k, 0., 1.);
	return mix(d2, -d1, h) + k * h * (1. - h);
}

mat2 r2(float r) { return mat2(cos(r), sin(r), -sin(r), cos(r)); }

vec4 texNoise(vec2 uv) {
	float f = 0.;
	f += texture(iChannel0, uv * .125).r * .5;
	f += texture(iChannel0, uv * .25).r * .25;
	f += texture(iChannel0, uv * .5).r * .125;
	f += texture(iChannel0, uv).r * .125;
	return vec4(pow(f, 1.2) * .45 + .05);
}

vec2 mp(vec3 p) {
	op = p;
	p.z = mod(p.z + tt * 6., 78.) - 39.;
	tp = p;
	p.xy *= r2(sin(op.z * .2 + tt * 2.) * .5);
	sp = p;
	float dis = sin(op.z * .2 + tt * 1.2) + sin(op.z * .1 + tt * .6) * .5,
	      zf = sin(p.z * 15.) * .03,
	      xf = sin(p.x * 15.) * .03,
	      tnoi = texNoise(p.yz * .1).r * .8,
	      fft = texture(iChannel1, vec2(min(abs(op.z * .002) * .15, 5.), 0)).x;
	sp.xy += dis;
	vec2 h,
	     t = vec2(length(sp.xy + xf) - (2. - dis + tnoi), 5);
	t.x = smin(t.x, length(abs(sp.xy + xf) - (1.5 - dis)) - (.5 + tnoi), .5);
	t.x = smin(t.x, length(abs(sp.xy + zf) - vec2(0, 2.1 - dis)) - (.5 + tnoi), .5);
	t.x = smax((length(abs(sp.xy + zf) - vec2(2. - dis, 0)) - (1. - dis + tnoi)), t.x, .3);
	hp = ep = sp;
	hp.z = mod(hp.z, 3.) - 1.5;
	t.x = smax(length(abs(hp - vec3(0, 0, .2)) - vec3(0, 2.1 - dis, 0)) - (.6 + tnoi - dis * .4), t.x, .5);
	hp.z -= min(pow(hp.y * .1, 2.) * 5., 5.);
	h = vec2(length(hp.xz + xf) - (.5 + tnoi - dis * .4 - abs(pow(hp.y * .09, 2.) * 4.)), 6);
	h.x *= .7;
	t = t.x < h.x ? t : h;
	sp.y *= .5;
	sp.z = mod(sp.z, 3.) - 1.5;
	h = vec2(length(sp) - max(.8, 1. - dis * .5), 6);
	g += .1 / (.1 + h.x * h.x * 40.);
	t = t.x < h.x ? t : h;
	for (int i = 0; i < 2; i++) {
		ep = abs(ep) - 2.;
		ep.xy *= r2(-.3);
		ep.yz *= r2(.5);
	}
	h = vec2(length(ep.xz + tnoi * 5.), 3);
	gg += .4 / (.1 + h.x * h.x + 1. * abs(sin(ep.y * .4 - tt * 2.)));
	t = t.x < h.x ? t : h;
	h = vec2(length(tp.xy) - 27., 6);
	tnoi = texNoise(vec2(abs(atan(tp.x, tp.y)), tp.z * .1 - 76.) * .07).r * 14.;
	h.x = max(h.x, -(length(tp.xy) - 14. + tnoi + fft * 3.));
	h.x = smax((length(abs(tp) - vec3(0, 15. - fft * 2., 0)) - (4. + tnoi)), h.x, 1.);
	h.x *= .8;
	t = t.x < h.x ? t : h;
	h = vec2((length(abs(tp) - vec3(0, 15. - fft * 2., 0)) - (3. + tnoi)), 7);
	gg += .4 / (.1 + h.x * h.x + 1. * abs(sin(ep.y * .4 - tt * 2. - .7)));
	t = t.x < h.x ? t : h;
	t.x *= .6;
	return t;
}

vec2 tr(vec3 ro, vec3 rd) {
	vec2 h,
	     t = vec2(.1);
	for (int i = 0; i < 128; i++) {
		h = mp(ro + rd * t.x);
		if (h.x < .00001 || t.x > 120.) break;
		t.x += h.x;
		t.y = h.y;
	}
	if (t.x > 120.) t.y = 0.;
	return t;
}

#define a(d)	clamp(mp(po + no * d).x / d, 0., 1.)
#define s(d)	smoothstep(0., 1., mp(po + ld * d).x / d)

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 uv = (fragCoord.xy / iResolution.xy - .5) / vec2(iResolution.y / iResolution.x, 1);
	tt = mod(iTime, 62.82);
	vec3 co, fo,
	     ro = vec3(cos(tt * .4) * 7., sin(tt * .4) * 7., -10),
	     cw = normalize(vec3(0, 0, cos(tt * .6) * 8.) - ro),
	     cu = normalize(cross(cw, vec3(sin(tt * .3) * .5, 1, 0))),
	     cv = normalize(cross(cu, cw)),
	     rd = mat3(cu, cv, cw) * normalize(vec3(uv, .5));
	ld = normalize(vec3(.2, .5, -.3));
	co = fo = vec3(.1) - length(uv) * .1;
	z = tr(ro, rd);
	t = z.x;
	if (z.y > 0.) {
		po = ro + rd * t;
		no = normalize(e.xyy * mp(po + e.xyy).x + e.yyx * mp(po + e.yyx).x + e.yxy * mp(po + e.yxy).x + e.xxx * mp(po + e.xxx).x);
		al = mix(vec3(.4, .5, .6), vec3(.6, .3, .2), .5 + .5 * sin(tp.z * .3 - 1.5));
		if (z.y < 5.) al = vec3(0);
		if (z.y > 5.) al = vec3(1);
		if (z.y > 6.) al = vec3(.1, .2, .4);
		float dif = max(0., dot(no, ld)),
		      fr = pow(1. + dot(no, rd), 4.),
		      sp = pow(max(dot(reflect(-ld, no), -rd), 0.), 50.);
		co = mix(fo, mix(sp + al * (a(.1) * a(.3) + .2) * (vec3(.5, .2, .1) * dif + s(.5) * 1.5), fo, min(fr, .5)), exp(-.00007 * t * t * t));
	}
	fragColor = vec4(pow(co + g * .2 * vec3(.5, .2, .1) + gg * .1 * vec3(.1, .2, .4), vec3(.55)), 1);
}