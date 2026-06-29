"""桌球游戏 - 物理碰撞模拟"""
import pygame
import math
import random

SCREEN_W, SCREEN_H = 900, 600
FELT = (15, 80, 25)
RAIL = (80, 50, 20)
POCKET_COLOR = (10, 10, 10)

# Pocket positions (6 pockets: 4 corners + 2 mid-sides)
POCKETS = [
    (45, 45), (SCREEN_W // 2, 30), (SCREEN_W - 45, 45),
    (45, SCREEN_H - 45), (SCREEN_W // 2, SCREEN_H - 30), (SCREEN_W - 45, SCREEN_H - 45),
]
POCKET_RADIUS = 24
BALL_RADIUS = 13
FRICTION = 0.985

BALL_COLORS = [
    (255, 255, 255),  # Cue ball
    (255, 215, 0),     # 1 - yellow
    (0, 100, 255),     # 2 - blue
    (255, 50, 50),     # 3 - red
    (180, 0, 220),     # 4 - purple
    (255, 120, 0),     # 5 - orange
    (0, 180, 80),      # 6 - green
    (150, 50, 50),     # 7 - maroon
    (30, 30, 30),      # 8 - black
]

INITIAL_TABLE = [
    (SCREEN_W // 2, SCREEN_H // 2, 8),  # 8-ball in center
    (SCREEN_W // 2 - 30, SCREEN_H // 2 - 17, 1),
    (SCREEN_W // 2 - 30, SCREEN_H // 2 + 17, 7),
    (SCREEN_W // 2 + 30, SCREEN_H // 2 - 17, 2),
    (SCREEN_W // 2 + 30, SCREEN_H // 2 + 17, 6),
    (SCREEN_W // 2 - 60, SCREEN_H // 2 - 34, 3),
    (SCREEN_W // 2 - 60, SCREEN_H // 2 + 34, 5),
    (SCREEN_W // 2 + 60, SCREEN_H // 2 - 34, 4),
]

class Ball:
    def __init__(self, x, y, color_idx):
        self.x = x
        self.y = y
        self.vx = 0.0
        self.vy = 0.0
        self.color = BALL_COLORS[color_idx]
        self.color_idx = color_idx
        self.active = True
        self.radius = BALL_RADIUS

    @property
    def speed(self):
        return math.hypot(self.vx, self.vy)

    def update(self):
        if not self.active:
            return
        self.x += self.vx
        self.y += self.vy
        self.vx *= FRICTION
        self.vy *= FRICTION
        if abs(self.vx) < 0.05:
            self.vx = 0
        if abs(self.vy) < 0.05:
            self.vy = 0

        # Rail collisions
        margin = RAIL_MARGIN + self.radius
        if self.x - self.radius < margin:
            self.x = margin + self.radius
            self.vx = abs(self.vx) * 0.8
        if self.x + self.radius > SCREEN_W - margin:
            self.x = SCREEN_W - margin - self.radius
            self.vx = -abs(self.vx) * 0.8
        if self.y - self.radius < margin:
            self.y = margin + self.radius
            self.vy = abs(self.vy) * 0.8
        if self.y + self.radius > SCREEN_H - margin:
            self.y = SCREEN_H - margin - self.radius
            self.vy = -abs(self.vy) * 0.8

    def pocket_check(self):
        for px, py in POCKETS:
            if math.hypot(self.x - px, self.y - py) < POCKET_RADIUS:
                self.active = False
                return True
        return False

    def draw(self, screen):
        if not self.active:
            return
        pygame.draw.circle(screen, self.color, (int(self.x), int(self.y)), self.radius)
        pygame.draw.circle(screen, (255, 255, 255, 80), (int(self.x - 3), int(self.y - 3)), self.radius // 3)
        pygame.draw.circle(screen, (0, 0, 0, 60), (int(self.x), int(self.y)), self.radius, 1)


RAIL_MARGIN = 45


class BilliardsGame:
    def __init__(self, screen, clock):
        self.screen = screen
        self.clock = clock
        self.font = pygame.font.Font(None, 28)
        self.font_big = pygame.font.Font(None, 42)
        self.reset()

    def reset(self):
        self.balls = []
        # Cue ball
        self.balls.append(Ball(SCREEN_W // 4, SCREEN_H // 2, 0))
        # Rack
        for x, y, c in INITIAL_TABLE:
            self.balls.append(Ball(x, y, c))
        self.cue_ball = self.balls[0]
        self.aiming = False
        self.aim_start = (0, 0)
        self.aim_end = (0, 0)
        self.shooting = False
        self.power = 0.0
        self.pocketed = 0
        self.win = False
        self.foul = ""

    def all_stopped(self):
        return all(b.speed < 0.1 for b in self.balls if b.active)

    def handle_ball_collisions(self):
        for i in range(len(self.balls)):
            if not self.balls[i].active:
                continue
            for j in range(i + 1, len(self.balls)):
                if not self.balls[j].active:
                    continue
                a, b = self.balls[i], self.balls[j]
                dx = a.x - b.x
                dy = a.y - b.y
                dist = math.hypot(dx, dy)
                min_dist = a.radius + b.radius
                if dist < min_dist and dist > 0:
                    # Separate
                    overlap = (min_dist - dist) / 2
                    nx, ny = dx / dist, dy / dist
                    a.x += nx * overlap
                    a.y += ny * overlap
                    b.x -= nx * overlap
                    b.y -= ny * overlap
                    # Elastic collision
                    dvx = a.vx - b.vx
                    dvy = a.vy - b.vy
                    dot = dvx * nx + dvy * ny
                    if dot > 0:
                        a.vx -= dot * nx
                        a.vy -= dot * ny
                        b.vx += dot * nx
                        b.vy += dot * ny

    def shoot(self):
        if self.power < 1:
            return
        dx = self.aim_start[0] - self.aim_end[0]
        dy = self.aim_start[1] - self.aim_end[1]
        dist = math.hypot(dx, dy)
        if dist > 0:
            force = min(self.power / 15, 18)
            self.cue_ball.vx = (dx / dist) * force
            self.cue_ball.vy = (dy / dist) * force
        self.shooting = True
        self.power = 0

    def draw_table(self):
        self.screen.fill((10, 10, 20))
        # Felt
        felt_rect = pygame.Rect(RAIL_MARGIN, RAIL_MARGIN, SCREEN_W - 2 * RAIL_MARGIN, SCREEN_H - 2 * RAIL_MARGIN)
        pygame.draw.rect(self.screen, FELT, felt_rect)
        # Rails
        pygame.draw.rect(self.screen, RAIL, felt_rect, 6)
        # Pockets
        for px, py in POCKETS:
            pygame.draw.circle(self.screen, POCKET_COLOR, (px, py), POCKET_RADIUS)
            pygame.draw.circle(self.screen, (40, 40, 40), (px, py), POCKET_RADIUS - 4)

    def draw_aim_line(self):
        if not self.aiming or self.shooting:
            return
        mx, my = pygame.mouse.get_pos()
        # Draw dotted aim line
        dx = self.cue_ball.x - mx
        dy = self.cue_ball.y - my
        length = math.hypot(dx, dy)
        if length > 5:
            nx, ny = dx / length, dy / length
            for dist in range(0, int(length), 10):
                px = int(self.cue_ball.x + nx * dist)
                py = int(self.cue_ball.y + ny * dist)
                if dist % 20 < 10:
                    pygame.draw.circle(self.screen, (255, 255, 255, 100), (px, py), 2)
            # Power meter
            meter_x = 20
            meter_h = 200
            meter_y = SCREEN_H // 2 - meter_h // 2
            power_pct = min(self.power / 250, 1.0)
            pygame.draw.rect(self.screen, (40, 40, 40), (meter_x - 2, meter_y - 2, 14, meter_h + 4))
            fill_h = int(meter_h * power_pct)
            color = (50, 255, 50) if power_pct < 0.5 else (255, 255, 50) if power_pct < 0.8 else (255, 50, 50)
            pygame.draw.rect(self.screen, color, (meter_x, meter_y + meter_h - fill_h, 10, fill_h))

    def draw_ui(self):
        status = f"Pocketed: {self.pocketed}/7  "
        if self.win:
            status = "YOU WIN! Press R to replay"
        elif self.foul:
            status = self.foul
        elif not self.all_stopped():
            status += "(moving...)"
        else:
            status += "Drag cue ball to aim, hold to charge"
        surf = self.font.render(status, True, (255, 255, 255))
        self.screen.blit(surf, (SCREEN_W // 2 - surf.get_width() // 2, 10))

    def run(self):
        self.reset()
        running = True
        while running:
            self.screen.fill((10, 10, 20))
            self.draw_table()

            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_ESCAPE:
                        running = False
                    if event.key == pygame.K_r:
                        self.reset()

                if self.all_stopped() and not self.shooting:
                    if event.type == pygame.MOUSEBUTTONDOWN:
                        mx, my = pygame.mouse.get_pos()
                        if math.hypot(mx - self.cue_ball.x, my - self.cue_ball.y) < 100:
                            self.aiming = True
                            self.aim_start = (self.cue_ball.x, self.cue_ball.y)
                            self.power = 0
                    if event.type == pygame.MOUSEBUTTONUP and self.aiming:
                        self.shoot()
                        self.aiming = False

            # Charge power while aiming
            if self.aiming:
                self.power += 2.5
                self.aim_end = pygame.mouse.get_pos()

            # Update balls
            for ball in self.balls:
                ball.update()
            self.handle_ball_collisions()

            # Check pockets
            for ball in self.balls:
                if ball is not self.cue_ball and ball.active and ball.pocket_check():
                    self.pocketed += 1
                    if self.pocketed >= 7:
                        self.win = True
                elif ball is self.cue_ball and not ball.active:
                    self.foul = "Foul! Cue ball pocketed. Press R to retry"

            if self.cue_ball.speed < 0.1 and not self.cue_ball.active:
                self.cue_ball.active = True
                self.cue_ball.x, self.cue_ball.y = SCREEN_W // 4, SCREEN_H // 2
                self.cue_ball.vx = self.cue_ball.vy = 0
                self.foul = ""

            # Clear shooting flag when all stop
            if self.shooting and self.all_stopped():
                self.shooting = False

            # Draw
            for ball in self.balls:
                ball.draw(self.screen)
            self.draw_aim_line()
            self.draw_ui()

            pygame.display.flip()
            self.clock.tick(60)

        self.reset()
