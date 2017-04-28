
var update = function(oldtime) {

  main_frame.update(new Date() * 0.001 - oldtime);

  main_frame.render();

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

  main_frame = new Frame(main_renderer);

  update();
}
