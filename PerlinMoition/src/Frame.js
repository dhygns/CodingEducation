

class Frame {
  constructor(rdrr) {
    this.rdrr = rdrr;

<<<<<<< HEAD
    this.particlemng = new ParticleUpdater(4096, rdrr);//16384 * 16, rdrr);
=======
    this.particlemng = new ParticleUpdater(8392, rdrr);//16384 * 16, rdrr);
>>>>>>> 4acc1a3c4a933819e4ab1e40fd4f7e8aa7c7213f
  }

  update(dt) {
    this.particlemng.update(dt);
  }

  render() {
    this.particlemng.render();
  }
}
