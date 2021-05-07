// Monster Things - Result of an improvised live code session on Twitch
// LIVE SHADER CODING, SHADER SHOWDOWN STYLE, EVERY TUESDAYS 21:00 Uk time: https://www.twitch.tv/evvvvil_
// Thankx to Haptix and canadianCornDog for the suggestions!
// Music by OddJohn / HANDSOME HOOLIGANS - https://soundcloud.com/oddjohnofficial

// "Motherfuckers stole my 80s style!" - Steven Spielberg

vec2 z,v,e=vec2(.01,-.01);float t,tt,g,gg;vec3 sp,tp,ep,op,hp,po,no,ld,al;//Global vars. About as boring as conspiracy theorists conspiring to make conspriacy theories
float smin(float a,float b,float h){float k=clamp((a-b)/h*.5+.5,0.,1.); return mix(a,b,k)-k*(1.-k)*h;} //Add geometries with smooth blend. Yeah this is what Sade talks about in the song "Smoothhhhhhh Operatooooor"
float smax(float d1,float d2,float k){float h=clamp(0.5-0.5*(d2+d1)/k,0.,1.);return mix(d2,-d1,h)+k*h*(1.0-h);}//Remove geometries with smooth blend. yeah sorry couldn't make a lamer joke than the one above...
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}//Rotate function. Short and sweet, just like how Kim Jung Un could be, if he had been given enough attention as a child.
vec4 texNoise(vec2 uv){ float f = 0.; f+=texture(iChannel0, uv*.125).r*.5; //Rough shadertoy approximation of the bonzomatic noise texture by yx - https://www.shadertoy.com/view/tdlXW4
    f+=texture(iChannel0,uv*.25).r*.25;f+=texture(iChannel0,uv*.5).r*.125;f+=texture(iChannel0,uv*1.).r*.125;f=pow(f,1.2);return vec4(f*.45+.05);
}
vec2 mp( vec3 p )
{ 
  op=p;//Remember original position in op
  p.z=mod(p.z+tt*6.,78.)-39.;//Modulo and animate everything else along the z axis
  tp=p;//Tunnel position is moduloed and animated (line above) but NOT twisted along z axis (next line)    
  p.xy*=r2(sin(op.z*.2+tt*2.)*.5);//Twist everything else along the z axis and animnate
  sp=p;//Sp is spine position used everywhere for monster
  float dis=sin(op.z*.2+tt*1.2)*1.+sin(op.z*.1+tt*.6)*.5; //dis is deformer / displacement along z axis with sin
  float zf=sin(p.z*15.)*0.03; //zf is z axis frill adding nice details like rib cage and crest
  float xf=sin(p.x*15.)*0.03; //xf is x axis frill adding nice details on edges etc
  float tnoi=texNoise(p.yz*.1).r*.8; //tnoi is texture noise displacement by sampling texture and making perlin, adds great curnchy organic details
  float fft=texture( iChannel1, vec2(min(abs(op.z*.002)*.15,5.),0)).x; //Audio sync along z limited to first 5 fft samples (bass)
  sp.xy+=dis;//add deformer to spine position
  vec2 h,t=vec2(length(sp.xy+xf)-(2.-dis+tnoi),5); //make main spline with deformer also influencing the radius not just position + texture displacement
  t.x=smin(t.x,length(abs(sp.xy+xf)-(1.5-dis))-(.5+tnoi),.5); //Add 4 cylinder to create edges again with deformer and texture displacement
  t.x=smin(t.x,length(abs(sp.xy+zf)-vec2(0,2.1-dis))-(.5+tnoi),.5);  //CREST made of two cylinder above and bellow with deformer and texture displacement
  t.x=smax((length(abs(sp.xy+zf)-vec2(2.-dis,0))-(1.-dis+tnoi)),t.x,.3);//RIBS made by carving out 2 cylinders on side and adding x axis frill
  hp=ep=sp; //horn positions and electricitings position
  hp.z=mod(hp.z,3.)-1.5; //modulo horn position along z to have loads of them on the crest
  t.x=smax(length(abs(hp-vec3(0,0,.2))-vec3(0,2.1-dis,0))-(.6+tnoi-dis*.4),t.x,.5); //HOLES FOR HORNS by reusing hp and shifting a bit
  hp.z-=min(pow(hp.y*.1,2.)*5.,5.);//HORN DEFORMER. Bend them forward menacingly. This could also be said in a BDSM scenario, involving people, also wearing horns.
  h=vec2(length(hp.xz+xf)-(.5+tnoi-dis*.4-abs(pow(hp.y*.09,2.)*4.)),6);  //HORNS made using hp postion and horn deformer
  h.x*=0.7; //We gotta increase the definition of horn geometry as they are deformed a lot, this avoids artifact
  t=t.x<h.x?t:h; //Add horns geometry to the scene while retinaing material id
  sp.y*=0.5;//EGGS position is reusing spine position but scaled along y to make spheres a taller egg shape
  sp.z=mod(sp.z,3.)-1.5; //Modulo along z to create many eggs along z axis, it's the same modulo frequency as for the horn position to align holes, horns and eggs. "Align holes, horns and eggs" sounds like a very good Peter Greenaway film!
  h=vec2(length(sp)-(max(.8,1.-dis*.5)),6);  //Make the actual eggs geometry and use displacement variable to change radius
  g+=0.1/(0.1+h.x*h.x*40.);//Add eggs distance field to red glow variable (added at the end)
  t=t.x<h.x?t:h;  //Add eggs to the scene
  for(int i=0;i<2;i++){//ELECTRICITINGS KIFS
   ep=abs(ep)-2.;//Each iter pull apart by 2 on xyz 
   ep.xy*=r2(-.3); //Rotate bit on XY
   ep.yz*=r2(.5);  //Rotate more on YZ
  }
  h=vec2(length(ep.xz+tnoi*5.),3);  //Make electricitings, please note the lack of radius, -0 in fact so ignored, that is how you get pure LASER. let's be pedantic about these things, this isn't Stepney Green Bingo Hall
  gg+=0.4/(0.1+h.x*h.x*1.+1.*abs(sin(ep.y*.4-tt*2.)));//Add electricitings distance field to blue glow variable (added at the end)
  t=t.x<h.x?t:h;  //Add electricitings to scene  
  h=vec2(length(tp.xy)-27.,6);  //OUTTER CYLINDER FOR TUNNEL
  tnoi=texNoise(vec2(abs(atan(tp.x,tp.y)),tp.z*.1-76.)*(.07)).r*14.;//REDEFINE texture noise with polar uvs
  h.x=max(h.x,-(length(tp.xy)-14.+tnoi+fft*3.)); //CUT HOLE IN TUNNEL
  h.x=smax((length(abs(tp)-vec3(0,15.-fft*2.,0))-(4.+tnoi)),h.x,1.);//HOLES FOR BLUE SPHERES
  h.x*=0.8; t=t.x<h.x?t:h; //Tweak disitance field to remove artifact and add to the scene
  h=vec2((length(abs(tp)-vec3(0,15.-fft*2.,0))-(3.+tnoi)),7);//BLUE SPHERES 
  gg+=0.4/(0.1+h.x*h.x*1.+1.*abs(sin(ep.y*.4-tt*2.-.7))); //Add blue spheres distance field to blue glow variable (added right at end)
  t=t.x<h.x?t:h; //Add blue spheres to scene 
  t.x*=0.6; return t;
}
vec2 tr( vec3 ro, vec3 rd) // main trace / raycast / raymarching loop function 
{
  vec2 h,t= vec2(.1); //Near plane because when it all started the hipsters still lived in Norwich and they only wore tweed.
  for(int i=0;i<128;i++){ //Main loop de loop 
    h=mp(ro+rd*t.x); //Marching forward like any good fascist army: without any care for culture theft. (get distance to geom)
    if(h.x<.00001||t.x>120.) break;//Conditional break we hit something or gone too far. Don't let the bastards break you down!
    t.x+=h.x;t.y=h.y; //Huge step forward and remember material id. Let me hold the bottle of gin while you count the colours.
  }
  if(t.x>120.) t.y=0.;//If we've gone too far then we stop, you know, like Alexander The Great did when he realised his wife was sexting some Turkish bloke. (10 points whoever gets the reference)
  return t;
}
#define a(d) clamp(mp(po+no*d).x/d,0.,1.)
#define s(d) smoothstep(0.,1.,mp(po+ld*d).x/d)
void mainImage( out vec4 fragColor, in vec2 fragCoord )//2 lines above are a = ambient occlusion and s = sub surface scattering
{
  vec2 uv=(fragCoord.xy/iResolution.xy-0.5)/vec2(iResolution.y/iResolution.x,1); //get UVs, nothing fancy, 
  tt=mod(iTime,62.82);  //Time variable, modulo'ed to avoid ugly artifact. Imagine moduloing your timeline, you would become a cry baby straight after dying a bitter old man. Christ, that's some fucking life you've lived, Steve.
  v=mix(vec2(3.,8.8),vec2(12.,16.),ceil(cos(tt*.4)));//Reuse the v variable as holder of camera variables
  vec3 ro=vec3(cos(tt*.4)*7.,sin(tt*.4)*7.,-10.),//Ro=ray origin=camera position We build camera right here broski. Gotta be able to see, to peep through the keyhole.
  cw=normalize(vec3(0,0,cos(tt*.6)*8.)-ro),cu=normalize(cross(cw,vec3(sin(tt*.3)*.5,1,0))),cv=normalize(cross(cu,cw)), //camera forward, left and up vector.
  rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo;//rd=ray direction (where the camera is pointing), co=final color, fo=fog color
  ld=normalize(vec3(.2,.5,-.3));//ld=light direction
  co=fo=vec3(.1)-length(uv)*.1;//background is dark with vignette
  z=tr(ro,rd);t=z.x; //Trace the trace in the loop de loop. Sow those fucking ray seeds and reap them fucking pixels.
  if(z.y>0.){ //Yeah we hit something, unlike you trying to throw a spear at a pig. We wouldnt have survive the ice age with you and your nerdy mates.
    po=ro+rd*t; //Get ray pos, know where you at, be where you is.
    no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x); //Make some fucking normals. You do the maths while I count how many brain cells I lost during my mid 2000s raving haydays.
    al=mix(vec3(.4,.5,.6),vec3(.6,.3,.2),.5+.5*sin(tp.z*.3-1.5));//albedo is base colour by default it's greyish to redish gradient
    if(z.y<5.) al=vec3(0); //material system if less than 5 make it black
    if(z.y>5.) al=vec3(1); //material system if more than 5 make it white
    if(z.y>6.)al=vec3(.1,.2,.4);//Material Id more than 6 makes it blue
    float dif=max(0.,dot(no,ld)), //Dumb as fuck diffuse lighting
    fr=pow(1.+dot(no,rd),4.), //Fr=fresnel which adds background reflections on edges to composite geometry better
    sp=pow(max(dot(reflect(-ld,no),-rd),0.),50.);//Sp=specular, stolen from shane
    co=mix(sp+al*(a(.1)*a(.3)+.2)*(vec3(.5,.2,.1)*dif+s(.5)*1.5),fo,min(fr,.5));//Building the final lighting result, compressing the fuck outta everything above into an RGB shit sandwich
    co=mix(fo,co,exp(-.00007*t*t*t)); //Fog soften things, but it won't stop your annoying uncle from thinking "Bloody fiddling with bloody numbers, ain't gonna get you a job, son. Real graft is what ye need, wee man."(last sentence read with Scottish accent if you can)
  }
  fragColor = vec4(pow(co+g*.2*vec3(.5,.2,.1)+gg*.1*vec3(.1,.2,.4),vec3(.55)),1); //Add glow at the end. g & gg are red and blue glow global variables containg distance fields see lines 42,50,58
}