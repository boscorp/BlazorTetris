window.Tetris2D = {
    canvas: null,
    nextCanvas: null,
    ctx: null,
    nextCtx: null,
    dotNetRef: null,
    blockSize: 30,
    boardWidth: 10,
    boardHeight: 20,
    
    initialize: function (canvasId, nextCanvasId, dotNetReference) {
        this.canvas = document.getElementById(canvasId);
        this.nextCanvas = document.getElementById(nextCanvasId);
        this.dotNetRef = dotNetReference;
        
        if (!this.canvas || !this.nextCanvas) {
            console.error('Canvas elements not found');
            return;
        }
        
        this.ctx = this.canvas.getContext('2d');
        this.nextCtx = this.nextCanvas.getContext('2d');
        
        // Set canvas size
        this.canvas.width = this.boardWidth * this.blockSize;
        this.canvas.height = this.boardHeight * this.blockSize;
        this.nextCanvas.width = 5 * this.blockSize; // Next piece preview
        this.nextCanvas.height = 5 * this.blockSize;
        
        this.setupEventListeners();
        this.draw();
        
        // Focus canvas for keyboard events
        this.canvas.setAttribute('tabindex', '0');
        this.canvas.focus();
    },
    
    setupEventListeners: function () {
        document.addEventListener('keydown', (event) => {
            if (this.dotNetRef) {
                this.dotNetRef.invokeMethodAsync('OnKeyDown', event.key);
            }
        });
    },
    
    draw: function () {
        // Clear main canvas with gradient background
        const gradient = this.ctx.createLinearGradient(0, 0, 0, this.canvas.height);
        gradient.addColorStop(0, '#001122');
        gradient.addColorStop(1, '#000011');
        this.ctx.fillStyle = gradient;
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw grid lines
        this.drawGrid();
        
        // Clear next piece canvas
        this.nextCtx.fillStyle = '#000011';
        this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
    },
    
    drawGrid: function () {
        this.ctx.strokeStyle = 'rgba(0, 255, 255, 0.2)';
        this.ctx.lineWidth = 0.5;
        
        // Vertical lines
        for (let x = 0; x <= this.boardWidth; x++) {
            this.ctx.beginPath();
            this.ctx.moveTo(x * this.blockSize, 0);
            this.ctx.lineTo(x * this.blockSize, this.canvas.height);
            this.ctx.stroke();
        }
        
        // Horizontal lines
        for (let y = 0; y <= this.boardHeight; y++) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, y * this.blockSize);
            this.ctx.lineTo(this.canvas.width, y * this.blockSize);
            this.ctx.stroke();
        }
        
        // Draw border
        this.ctx.strokeStyle = 'rgba(0, 255, 255, 0.8)';
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(0, 0, this.canvas.width, this.canvas.height);
    },
    
    drawBlock: function (ctx, x, y, color, blockSize = this.blockSize) {
        // Validate coordinates
        if (!Number.isFinite(x) || !Number.isFinite(y) || x < 0 || y < 0 || x >= this.boardWidth || y >= this.boardHeight) {
            console.warn('Invalid block coordinates:', x, y);
            return;
        }
        
        const blockX = x * blockSize;
        const blockY = y * blockSize;
        const padding = 1;
        const innerSize = blockSize - (padding * 2);
        
        // Create gradient for the block
        const gradient = ctx.createLinearGradient(blockX, blockY, blockX + blockSize, blockY + blockSize);
        gradient.addColorStop(0, this.lightenColor(color, 0.3));
        gradient.addColorStop(1, this.darkenColor(color, 0.2));
        
        // Draw main block
        ctx.fillStyle = gradient;
        ctx.fillRect(blockX + padding, blockY + padding, innerSize, innerSize);
        
        // Draw highlight
        ctx.fillStyle = 'rgba(255, 255, 255, 0.4)';
        ctx.fillRect(blockX + padding + 2, blockY + padding + 2, innerSize - 4, 4);
        ctx.fillRect(blockX + padding + 2, blockY + padding + 2, 4, innerSize - 4);
        
        // Draw border
        ctx.strokeStyle = this.darkenColor(color, 0.4);
        ctx.lineWidth = 1;
        ctx.strokeRect(blockX + padding, blockY + padding, innerSize, innerSize);
    },
    
    lightenColor: function(color, amount) {
        const num = parseInt(color.replace("#",""), 16);
        const amt = Math.round(2.55 * amount * 100);
        const R = (num >> 16) + amt;
        const G = (num >> 8 & 0x00FF) + amt;
        const B = (num & 0x0000FF) + amt;
        return "#" + (0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 +
            (G < 255 ? G < 1 ? 0 : G : 255) * 0x100 +
            (B < 255 ? B < 1 ? 0 : B : 255)).toString(16).slice(1);
    },
    
    darkenColor: function(color, amount) {
        const num = parseInt(color.replace("#",""), 16);
        const amt = Math.round(2.55 * amount * 100);
        const R = (num >> 16) - amt;
        const G = (num >> 8 & 0x00FF) - amt;
        const B = (num & 0x0000FF) - amt;
        return "#" + (0x1000000 + (R > 255 ? 255 : R < 0 ? 0 : R) * 0x10000 +
            (G > 255 ? 255 : G < 0 ? 0 : G) * 0x100 +
            (B > 255 ? 255 : B < 0 ? 0 : B)).toString(16).slice(1);
    },
    
    updateBoard: function (filledBlocks, currentBlocks, currentColor, nextBlocks, nextColor) {
        // Clear and redraw
        this.draw();
        
        // Draw placed blocks
        if (filledBlocks && filledBlocks.length > 0) {
            filledBlocks.forEach(blockData => {
                const x = blockData.position?.x ?? blockData.Position?.X;
                const y = blockData.position?.y ?? blockData.Position?.Y;
                const color = blockData.color || blockData.Color;
                
                if (x !== undefined && y !== undefined && x >= 0 && x < this.boardWidth && y >= 0 && y < this.boardHeight) {
                    // Convert game coordinates (Y=0 bottom) to canvas coordinates (Y=0 top)
                    const canvasY = this.boardHeight - 1 - y;
                    this.drawBlock(this.ctx, x, canvasY, color);
                }
            });
        }
        
        // Draw current piece
        if (currentBlocks && currentBlocks.length > 0) {
            currentBlocks.forEach(block => {
                const x = block.x ?? block.X;
                const y = block.y ?? block.Y;
                
                if (x !== undefined && y !== undefined && x >= 0 && x < this.boardWidth && y >= 0 && y < this.boardHeight) {
                    // Convert game coordinates (Y=0 bottom) to canvas coordinates (Y=0 top)
                    const canvasY = this.boardHeight - 1 - y;
                    this.drawBlock(this.ctx, x, canvasY, currentColor);
                }
            });
        }
        
        // Draw next piece
        if (nextBlocks && nextBlocks.length > 0) {
            // Clear next canvas
            this.nextCtx.fillStyle = '#000011';
            this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
            
            // Find bounds of next piece for centering
            const minX = Math.min(...nextBlocks.map(b => b.x ?? b.X));
            const maxX = Math.max(...nextBlocks.map(b => b.x ?? b.X));
            const minY = Math.min(...nextBlocks.map(b => b.y ?? b.Y));
            const maxY = Math.max(...nextBlocks.map(b => b.y ?? b.Y));
            
            const pieceWidth = maxX - minX + 1;
            const pieceHeight = maxY - minY + 1;
            const offsetX = (5 - pieceWidth) / 2;
            const offsetY = (5 - pieceHeight) / 2;
            
            nextBlocks.forEach(block => {
                const x = (block.x ?? block.X) - minX + offsetX;
                const y = (block.y ?? block.Y) - minY + offsetY;
                this.drawBlock(this.nextCtx, x, y, nextColor, this.blockSize * 0.7);
            });
        }
    },
    
    showGameOver: function () {
        // Draw game over overlay
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.9)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Game Over text with glow effect
        this.ctx.shadowColor = '#ff0000';
        this.ctx.shadowBlur = 20;
        this.ctx.fillStyle = '#ff4444';
        this.ctx.font = 'bold 32px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText('GAME OVER', this.canvas.width / 2, this.canvas.height / 2);
        
        this.ctx.shadowBlur = 0;
        this.ctx.fillStyle = '#ffffff';
        this.ctx.font = '16px Arial';
        this.ctx.fillText('Press Start to play again', this.canvas.width / 2, this.canvas.height / 2 + 40);
    },
    
    focusCanvas: function () {
        if (this.canvas) {
            this.canvas.focus();
        }
    },
    
    dispose: function () {
        // Clean up event listeners if needed
        if (this.canvas) {
            this.canvas.removeEventListener('keydown', this.handleKeyPress);
        }
    }
};