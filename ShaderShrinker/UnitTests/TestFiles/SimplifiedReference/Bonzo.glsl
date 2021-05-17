#version 410 core

precision mediump float;
uniform float fGlobalTime;
uniform vec2 v2Resolution;
uniform sampler1D texFFT;
layout(location = 0)out vec4 out_color;
vec4 plas(vec2 v, float time) {
	float c = .5 + sin(v.x * 10.) + cos(sin(time + v.y) * 20.);
	return vec4(sin(c * .2 + cos(time)), c * .15, cos(c * .1 + time / .4) * .25, 1);
}

void main() {
	vec2 m,
	     uv = vec2(gl_FragCoord.x / v2Resolution.x, gl_FragCoord.y / v2Resolution.y);
	uv -= .5;
	uv /= vec2(v2Resolution.y / v2Resolution.x, 1);
	m.x = atan(uv.x / uv.y) / 3.14;
	m.y = 1 / length(uv) * .2;
	float d = m.y,
	      f = texture(texFFT, d).r * 100;
	m.x += sin(fGlobalTime) * .1;
	m.y += fGlobalTime * .25;
	out_color = f + clamp(plas(m * 3.14, fGlobalTime) / d, 0., 1.);
}