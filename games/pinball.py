"""弹珠台游戏 - 经典街机弹球"""
import pygame
import math
import random

SCREEN_W, SCREEN_H = 500, 700
BG_COLOR = (10, 10, 30)
FLIPPER_COLOR = (220, 180, 80)
BALL_COLOR = (255, 50, 50)
BUMPER_COLOR = (255, 140, 0)
WALL_COLOR = (100, 100, 180)

GRAVITY = 0.25
BALL_RADIUS = 10
FLIPPER_LENGTH = 70
FLIPPER_WIDTH = 14


class Flipper:
    def __init__(self, x, y, is_left):
        self.x = x
        self.y = y
        self.is_left = is_left
        self.angle = 30 if is_left else 150  # degrees, resting
        self.rest_angle = 30 if is_left else 150
        self.active_angle = 0 if is_left else 180
        self.angular_vel = 0

    def activate(self):
        target = self.active_angle
        diff = target - self.angle
        if abs(diff) < 1:
            self.angle = target
        elif diff > 0:
            self.angle = min(self.angle + 8, target)
        else:
            self.angle = max(self.angle - 8, target)

    def deactivate(self):
        target = self.rest_angle
        diff = target - self.angle
        if abs(diff) < 1:
            self.angle = target
        elif diff > 0:
            self.angle = min(self.angle + 3, target)
        else:
            self.angle = max(self.angle - 3, target)

    def get_endpoints(self):
        rad = math.radians(self.angle)
        if self.is_left:
            ex = self.x + FLIPPER_LENGTH * math.cos(rad)
            ey = self.y - FLIPPER_LENGTH * math.sin(rad)
        else:
            ex = self.x - FLIPPER_LENGTH * math.cos(math.radians(180 - self.angle))
            ey = self.y - FLIPPER_LENGTH * math.sin(math.radians(180 - self.angle))
        return (self.x, self.y), (ex, ey)


class PinballGame:
    def __init__(self, screen, clock):
        self.screen = screen
        self.clock = clock
        self.font = pygame.font.Font(None, 32)
        self.font_big = pygame.font.Font(None, 48)
        self.font_small = pygame.font.Font(None, 22)
        self.reset()

    def reset(self):
        self.ball_x = SCREEN_W // 2
        self.ball_y = SCREEN_H - 150
        self.ball_vx = random.uniform(-2, 2)
        self.ball_vy = random.uniform(-4, -2)
        self.score = 0
        self.balls_left = 3
        self.game_over = False
        self.flipper_left = Flipper(140, SCREEN_H - 60, True)
        self.flipper_right = Flipper(360, SCREEN_H - 60, False)
        self.flippers_active = False

        # Bumpers (circular bumpers that give points on hit)
        self.bumpers = []
        for i in range(5):
            bx = 100 + random.randint(0, SCREEN_W - 200)
            by = 100 + random.randint(0, SCREEN_H - 400)
            self.bumpers.append({"x": bx, "y": by, "r": 18, "active": True, "hit_timer": 0})

        # Score zones
        self.score_zones = [
            {"x": 60, "y": 150, "w": 80, "h": 30, "score": 50, "color": (0, 200, 100)},
            {"x": SCREEN_W - 140, "y": 150, "w": 80, "h": 30, "score": 50, "color": (0, 200, 100)},
            {"x": SCREEN_W // 2 - 50, "y": 100, "w": 100, "h": 25, "score": 100, "color": (200, 50, 200)},
        ]

    def handle_wall_collisions(self):
        # Side walls
        if self.ball_x - BALL_RADIUS < 20:
            self.ball_x = 20 + BALL_RADIUS
            self.ball_vx = abs(self.ball_vx) * 0.8
        if self.ball_x + BALL_RADIUS > SCREEN_W - 20:
            self.ball_x = SCREEN_W - 20 - BALL_RADIUS
            self.ball_vx = -abs(self.ball_vx) * 0.8
        # Top
        if self.ball_y - BALL_RADIUS < 20:
            self.ball_y = 20 + BALL_RADIUS
            self.ball_vy = abs(self.ball_vy) * 0.8

    def handle_flipper_collisions(self):
        for flipper in [self.flipper_left, self.flipper_right]:
            p1, p2 = flipper.get_endpoints()
            # Simple line-circle collision
            dx = p2[0] - p1[0]
            dy = p2[1] - p1[1]
            fx = self.ball_x - p1[0]
            fy = self.ball_y - p1[1]
            dot = max(0, min(1, (fx * dx + fy * dy) / (dx * dx + dy * dy + 0.001)))
            closest_x = p1[0] + dot * dx
            closest_y = p1[1] + dot * dy
            dist = math.hypot(self.ball_x - closest_x, self.ball_y - closest_y)
            if dist < BALL_RADIUS + FLIPPER_WIDTH // 2:
                # Push ball away
                nx = (self.ball_x - closest_x) / (dist + 0.001)
                ny = (self.ball_y - closest_y) / (dist + 0.001)
                self.ball_x = closest_x + nx * (BALL_RADIUS + FLIPPER_WIDTH // 2)
                self.ball_y = closest_y + ny * (BALL_RADIUS + FLIPPER_WIDTH // 2)
                speed = math.hypot(self.ball_vx, self.ball_vy)
                self.ball_vx = nx * max(speed, 5)
                self.ball_vy = ny * max(speed, 5) - 3

    def handle_bumper_collisions(self):
        for b in self.bumpers:
            if not b["active"]:
                continue
            dist = math.hypot(self.ball_x - b["x"], self.ball_y - b["y"])
            if dist < BALL_RADIUS + b["r"]:
                nx = (self.ball_x - b["x"]) / dist
                ny = (self.ball_y - b["y"]) / dist
                self.ball_x = b["x"] + nx * (BALL_RADIUS + b["r"])
                self.ball_y = b["y"] + ny * (BALL_RADIUS + b["r"])
                self.ball_vx = nx * 6
                self.ball_vy = ny * 6
                self.score += 25
                b["hit_timer"] = 10

    def handle_score_zones(self):
        for z in self.score_zones:
            if (z["x"] < self.ball_x < z["x"] + z["w"] and
                z["y"] < self.ball_y < z["y"] + z["h"]):
                self.score += z["score"]
                self.ball_vy = -abs(self.ball_vy) * 1.1

    def update(self):
        if self.game_over:
            return

        self.ball_vy += GRAVITY
        self.ball_x += self.ball_vx
        self.ball_y += self.ball_vy

        self.handle_wall_collisions()
        self.handle_bumper_collisions()
        self.handle_score_zones()
        self.handle_flipper_collisions()

        # Bumper timers
        for b in self.bumpers:
            if b["hit_timer"] > 0:
                b["hit_timer"] -= 1

        # Ball lost
        if self.ball_y > SCREEN_H + 20:
            self.balls_left -= 1
            if self.balls_left <= 0:
                self.game_over = True
            else:
                self.ball_x = SCREEN_W // 2
                self.ball_y = SCREEN_H - 150
                self.ball_vx = random.uniform(-2, 2)
                self.ball_vy = random.uniform(-4, -2)

    def draw_flipper(self, flipper):
        p1, p2 = flipper.get_endpoints()
        pygame.draw.line(self.screen, FLIPPER_COLOR, p1, p2, FLIPPER_WIDTH)
        pygame.draw.circle(self.screen, FLIPPER_COLOR, (int(p1[0]), int(p1[1])), 10)
        pygame.draw.circle(self.screen, FLIPPER_COLOR, (int(p2[0]), int(p2[1])), 7)

    def run(self):
        self.reset()
        running = True
        while running:
            self.screen.fill(BG_COLOR)

            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_ESCAPE:
                        running = False
                    if event.key == pygame.K_r and self.game_over:
                        self.reset()
                    if event.key in (pygame.K_SPACE, pygame.K_LEFT, pygame.K_RIGHT):
                        self.flippers_active = True
                if event.type == pygame.KEYUP:
                    if event.key in (pygame.K_SPACE, pygame.K_LEFT, pygame.K_RIGHT):
                        self.flippers_active = False

            keys = pygame.key.get_pressed()
            self.flippers_active = keys[pygame.K_SPACE] or keys[pygame.K_LEFT] or keys[pygame.K_RIGHT]

            if self.flippers_active:
                self.flipper_left.activate()
                self.flipper_right.activate()
            else:
                self.flipper_left.deactivate()
                self.flipper_right.deactivate()

            self.update()

            # Draw walls
            pygame.draw.rect(self.screen, WALL_COLOR, (10, 10, SCREEN_W - 20, SCREEN_H - 20), 4)
            # Guide rails
            pygame.draw.line(self.screen, WALL_COLOR, (20, 80), (80, 160), 4)
            pygame.draw.line(self.screen, WALL_COLOR, (SCREEN_W - 20, 80), (SCREEN_W - 80, 160), 4)

            # Bumpers
            for b in self.bumpers:
                color = (255, 255, 150) if b["hit_timer"] > 0 else BUMPER_COLOR
                r = b["r"] + 3 if b["hit_timer"] > 0 else b["r"]
                pygame.draw.circle(self.screen, color, (b["x"], b["y"]), r)
                pygame.draw.circle(self.screen, (255, 200, 100), (b["x"], b["y"]), r - 5, 2)

            # Score zones
            for z in self.score_zones:
                pygame.draw.rect(self.screen, z["color"], (z["x"], z["y"], z["w"], z["h"]), border_radius=4)
                txt = self.font_small.render(str(z["score"]), True, (255, 255, 255))
                self.screen.blit(txt, (z["x"] + z["w"] // 2 - txt.get_width() // 2, z["y"] + 4))

            # Flippers
            self.draw_flipper(self.flipper_left)
            self.draw_flipper(self.flipper_right)

            # Ball
            pygame.draw.circle(self.screen, BALL_COLOR, (int(self.ball_x), int(self.ball_y)), BALL_RADIUS)
            pygame.draw.circle(self.screen, (255, 200, 200), (int(self.ball_x - 2), int(self.ball_y - 2)), BALL_RADIUS // 3)

            # UI
            score_surf = self.font.render(f"Score: {self.score}", True, (255, 255, 255))
            balls_surf = self.font.render(f"Balls: {self.balls_left}", True, (255, 255, 255))
            self.screen.blit(score_surf, (30, SCREEN_H - 30))
            self.screen.blit(balls_surf, (SCREEN_W - 150, SCREEN_H - 30))

            if self.game_over:
                go_surf = self.font_big.render("GAME OVER", True, (255, 50, 50))
                hint = self.font.render("Press R to restart", True, (200, 200, 200))
                self.screen.blit(go_surf, (SCREEN_W // 2 - go_surf.get_width() // 2, SCREEN_H // 2 - 40))
                self.screen.blit(hint, (SCREEN_W // 2 - hint.get_width() // 2, SCREEN_H // 2 + 20))

            hint_surf = self.font_small.render("Hold SPACE for flippers | ESC to exit", True, (150, 150, 150))
            self.screen.blit(hint_surf, (SCREEN_W // 2 - hint_surf.get_width() // 2, SCREEN_H - 30))

            pygame.display.flip()
            self.clock.tick(60)

        self.reset()
