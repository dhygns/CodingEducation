

class Frame {
  constructor(rdrr) {
    this.rdrr = rdrr;

    this.particlemng = new ParticleUpdater(16384 * 16, rdrr);
  }

  update(dt) {
    this.particlemng.update(dt);
  }

  render() {
    this.particlemng.render();
  }
}
