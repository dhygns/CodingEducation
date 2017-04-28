
var update = function(oldtime) {

  main_frame.update(new Date() * 0.001 - oldtime);

  main_renderer.render(main_frame.scene, main_frame.camera, main_canvas);
  main_renderer.render(main_scene, main_camera);

  requestAnimationFrame(update.bind(null, new Date() * 0.001));
}

var main = function() {
  main_renderer = new THREE.WebGLRenderer({
  // preserveDrawingBuffer : true,
    alpha : true,
  });
  main_renderer.setSize(window.innerWidth, window.innerHeight);
  main_renderer.autoClear = false;
  document.body.appendChild(main_renderer.domElement);

  main_canvas = new THREE.WebGLRenderTarget( window.innerWidth, window.innerHeight, {
    minFilter : THREE.minFilter,
    magFilter : THREE.magFilter,
  });
  main_scene = new THREE.Scene();
  main_camera = new THREE.Camera();
  main_plane = new THREE.Mesh(
    new THREE.PlaneGeometry(2.0, 2.0),
    new THREE.ShaderMaterial({
      uniforms : { unif_texture : { type : "t", value : main_canvas.texture}},
      vertexShader : `
      varying vec2 vtex;
      void main(void) {
        vtex = uv;
        gl_Position = vec4(position, 1.0);
      }
      `,
      fragmentShader : `
      uniform sampler2D unif_texture;
      varying vec2 vtex;
      void main(void) {
        vec4 retcol = texture2D(unif_texture, vtex);
        float brightness = smoothstep( 0.55 - 0.01, 0.55 + 0.01, retcol.a);
        gl_FragColor = vec4(0.0, 0.0, 0.0, brightness);
      }
      `

  }));
  main_scene.add(main_plane);

  main_frame = new Frame();

  update();
}
