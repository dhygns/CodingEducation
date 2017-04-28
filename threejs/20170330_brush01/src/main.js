
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
    new THREE.MeshBasicMaterial({map : main_canvas.texture})
  );
  main_scene.add(main_plane);

  main_frame = new Frame();

  update();
}
