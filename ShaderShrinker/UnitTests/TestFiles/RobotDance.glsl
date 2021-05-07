// blackle - https://www.shadertoy.com/view/7sS3WD

float box(vec3 p, vec3 d) {
  p = abs(p) - d;
  return length(max(p,0.)) + min(0.,max(max(p.x,p.y),p.z));
}

float joint(vec3 p, vec3 d) {
  return min(length(p - vec3(0,0,d.z))-d.y*1.5-.02, box(p,d)-.02);
}

vec3 erot(vec3 p, vec3 ax, float ro) {
  return mix(dot(ax,p)*ax, p, cos(ro)) + sin(ro)*cross(ax,p);
}

float r1;
float r2;
float r3;
float r4;
float r5;
float r6;
float arm;
float fl;
float scene(vec3 p) {
  float dist = joint(p, vec3(.1,.1,.4));
  fl = p.z+.4;
  dist = min(dist,fl);
  p = erot(erot(p,vec3(0,0,1),r2)-vec3(0.,0.,.4), vec3(1,0,0), r1) - vec3(0,0,0.3);
  dist = min(dist, joint(p, vec3(.1,.1,.3)));
  p = erot(erot(p,vec3(0,0,1),r4)-vec3(0.,0.,.3), vec3(1,0,0), r3) - vec3(0,0,0.3);
  dist = min(dist, joint(p, vec3(.1,.1,.3)));
  p -= vec3(0,0,.32);
  p = erot(p,vec3(0,0,1),r6);
  arm = box(p, vec3(.09,.3,.09))-.02;
  p.y = abs(p.y);
  p -= vec3(0,.3,.12);
  p = erot(p-vec3(0,0,-.2), vec3(1,0,0), r5);
  arm = min(arm, length(p+vec3(0,0,-.1))-.15);
  arm = min(arm, joint(p-vec3(0,0,.3), vec3(.05,.05,.2)));
  return min(dist,arm);
}

#define FK(k) floatBitsToInt(k*k/7.)^floatBitsToInt(k)
float hash(float a, float b) {
  int x = FK(a), y = FK(b);
  return float((x*x-y)*(y*y+x)+x)/2.14e9;
}

float spring(float x) {
  return smoothstep(-.1,.1,x) + smoothstep(-.2,.2,x) - smoothstep(-.3,.3,x);
}

float bpm = 130.;
float mayhem(float sd) {
  float off = hash(sd,sd)*99.;
  float idx = round((iTime+off)*bpm/240.);
  float interp = iTime + off - idx*240./bpm;
  float start = hash(idx, sd);
  float end = hash(round(idx+1.), sd);
  return mix(start, end, spring(interp));
}

float revisionTexture(vec2 p) {
    return step(0., sin(length(p*40.)+atan(p.x,p.y)));
}

vec3 norm(vec3 p) {
  mat3 k = mat3(p,p,p) - mat3(0.001);
  return normalize(scene(p) - vec3(scene(k[0]),scene(k[1]),scene(k[2])));
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = (fragCoord-.5*iResolution.xy)/iResolution.y;

      r1 = mayhem(12.);
  r3 = mayhem(32.);
  r5 = mayhem(72.)*.5+.5;
  r2 = mayhem(92.);
  r4 = mayhem(78.);
  r6 = mayhem(89.);
  float hs = mayhem(45.);
  
  vec3 cam = normalize(vec3(1.+hs*.5+.5,uv));
  vec3 init = vec3(-5,0,.5+hs*.2);
  
  float yrot = .5+mayhem(85.)*.3;
  float zrot = iTime+mayhem(98.);
  cam = erot(cam, vec3(0,1,0), yrot);
  init = erot(init, vec3(0,1,0), yrot);
  cam = erot(cam, vec3(0,0,1), zrot);
  init = erot(init, vec3(0,0,1), zrot);
  
  vec3 p = init;
  bool hit = false;
  float dist;
  for (int i = 0; i<150 && !hit; i++) {
    dist = scene(p);
    hit = dist*dist < 1e-6;
    p += cam*dist;
    if (distance(p,init)>10.) break;
  }
  bool isfl = fl == dist;
  bool isarm = arm == dist;
  vec3 n = norm(p);
  vec3 r = reflect(cam,n);
  float fres = 1. - abs(dot(cam,n))*.98;
  float spec = length(sin(r*3.)*.5+.5)/length(3.);
  spec = pow(spec, 3.)*2. + spec*.2;
  spec *= fres;
  vec3 col = vec3(spec);
  if (isarm) {
    col += vec3(.8,.3,.1)*length(sin(n*2.)*.3+.7)/sqrt(3.);
  }
  if (isfl) {
    col = vec3(1);
    if(length(p.xy) < 3.) {
      if (length(p.xy) < 1.5) {
        p = erot(p, vec3(0,0,1), mayhem(78.));
      } else {
        p = erot(p, vec3(0,0,1), mayhem(99.));
      }
      col = vec3(revisionTexture(p.xy/6.));
    }
  }

	fragColor.xyz = smoothstep(0.,1.,sqrt(hit ? col : vec3(1)));
}